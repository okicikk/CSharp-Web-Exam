using DeskMarket.Data;
using DeskMarket.Data.Models;
using DeskMarket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Drawing.Printing;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using static DeskMarket.Constants.Constants;

namespace DeskMarket.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        public ProductController(ApplicationDbContext _context)
        {
            context = _context;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ProductIndexViewModel> models = new();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var p in context.Products
                .Include(p=>p.ProdcutsClients)
                .ThenInclude(pc=>pc.Client)
                .Where(p => p.IsDeleted == false))
            {
                bool hasBought = p.ProdcutsClients.Any(pc => pc.ClientId == userId && pc.ProductId == p.Id);

                ProductIndexViewModel model = new ProductIndexViewModel()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsSeller = userId == p.SellerId,
                    HasBought = hasBought,
                };
                models.Add(model);
                

            }

            return View(models);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ProductClient productClient = new ProductClient()
            {
                ClientId = userId,
                ProductId = id,
            };
            if (!context.ProductsClients.Any(pc => pc == productClient))
            {
                await context.ProductsClients.AddAsync(productClient);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ProductAddViewModel viewModel = new ProductAddViewModel()
            {
                Categories = await context.Categories.ToListAsync(),
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await context.Categories.ToListAsync();
                return View(model);
            }
            if (!DateTime.TryParseExact(model.AddedOn, ProperDateFormat
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None
                , out DateTime addedOnDateTime))
            {
                model.Categories = await context.Categories.ToListAsync();
                return View(model);
            }
            Product product = new Product()
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                SellerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AddedOn = addedOnDateTime,
                CategoryId = model.CategoryId,
                IsDeleted = false
            };

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            List<ProductCartViewModel> models = new List<ProductCartViewModel>();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            foreach (var p in context.ProductsClients
                .Include(p => p.Product)
                .Where(pc => pc.ClientId == userId))
            {
                if (p.Product.IsDeleted == false)
                {
                    ProductCartViewModel model = new ProductCartViewModel
                    {
                        Id = p.ProductId,
                        ProductName = p.Product.ProductName,
                        Price = p.Product.Price,
                        ImageUrl = p.Product.ImageUrl
                    };
                    models.Add(model);
                }
            }
            return View(models);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await context
                .Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product.IsDeleted == true || product.SellerId != FindCurrentUserId())
            {
                return RedirectToAction(nameof(Index));
            }

            ProductEditViewModel model = new()
            {
                Id = id,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                AddedOn = product.AddedOn.ToString(ProperDateFormat),
                CategoryId = product.CategoryId,
                Categories = context.Categories.ToList(),
                SellerId = product.SellerId
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Categories = context.Categories.ToList();
                return View(model);
            }

            Product productToBeEdited = await context.Products.FirstOrDefaultAsync(p => p.Id == model.Id);

            if (!DateTime.TryParseExact(model.AddedOn, ProperDateFormat
                , CultureInfo.InvariantCulture
                , DateTimeStyles.None
                , out DateTime addedOnDateTime))
            {
                return View(model);
            }

            productToBeEdited.ProductName = model.ProductName;
            productToBeEdited.Price = model.Price;
            productToBeEdited.Description = model.Description;
            productToBeEdited.ImageUrl = model.ImageUrl;
            productToBeEdited.AddedOn = addedOnDateTime;
            productToBeEdited.CategoryId = model.CategoryId;


            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = productToBeEdited.Id });

        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Product? product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return RedirectToAction(nameof(Index));
            }

            ProductClient? productClient = await context
                .ProductsClients
                .FirstOrDefaultAsync(pc => pc.ProductId == product.Id && pc.ClientId == userId);

            if (productClient is null)
            {
                return RedirectToAction(nameof(Cart));
            }

            context.ProductsClients.Remove(productClient);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product.IsDeleted == true)
            {
                return RedirectToAction(nameof(Index));
            }
            ProductDetailsModelView model = new ProductDetailsModelView()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                Description = product.Description,
                CategoryName = product.Category.Name,
                AddedOn = product.AddedOn.ToString(ProperDateFormat),
                Seller = product.Seller.UserName ?? "Unknown",
                HasBought = context.ProductsClients.Any(pc => pc.ProductId == id && pc.ClientId == userId),
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = context
                .Products
                .Include(p => p.Seller)
                .FirstOrDefault(p => p.Id == id);


            var userId = FindCurrentUserId();

            DeleteProductViewModel viewModel = new DeleteProductViewModel()
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Seller = product.Seller.UserName,
                SellerId = product.SellerId
            };

            if (product.Seller.Id == userId)
            {
                return View(viewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteProductViewModel model)
        {
            int id = model.Id;

            var userId = FindCurrentUserId();

            Product? product = await context
                .Products
                .FirstOrDefaultAsync(p => p.Id == id && p.SellerId == userId);



            if (product is null || product.IsDeleted)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var pc in context.ProductsClients.Where(pc => pc.ProductId == id))
            {
                context.ProductsClients.Remove(pc);
            }

            product.IsDeleted = true;
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        private string FindCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
