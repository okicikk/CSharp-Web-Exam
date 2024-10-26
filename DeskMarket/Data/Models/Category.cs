using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.Constants;
namespace DeskMarket.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(CategoryMaxName)]
        public string Name { get; set; }
        public IList<Product> ProductsClients { get; set; } = new List<Product>();

    }
}
