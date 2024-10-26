using DeskMarket.Data.Models;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.Constants;
namespace DeskMarket.Models
{
    public class ProductEditViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; }
        [Required]
        [Range((double)PriceMinNumber, (double)PriceMaxNumber)]
        public decimal Price { get; set; }
        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string AddedOn { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public string SellerId { get; set; }
        public IList<Category> Categories { get; set; } = new List<Category>();
    }
}
