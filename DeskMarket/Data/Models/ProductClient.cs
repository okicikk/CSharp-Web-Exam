using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DeskMarket.Data.Models
{
    //[PrimaryKey(nameof(ProductId), nameof(ClientId))]
    public class ProductClient
    {
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public string ClientId { get; set; }
        public IdentityUser Client { get; set; }
    }
}
