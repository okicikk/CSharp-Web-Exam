using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.Constants;
namespace DeskMarket.Data.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; }
        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }
        [Required]
        [Range((double)PriceMinNumber, (double)PriceMaxNumber)]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string SellerId { get; set; }
        [Required]
        public IdentityUser Seller { get; set; }
        [Required]
        public DateTime AddedOn { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public Category Category { get; set; }
        public bool IsDeleted { get; set; } = false;
        public IList<ProductClient> ProdcutsClients { get; set; } = new List<ProductClient>();
    }
}
