﻿using DeskMarket.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static DeskMarket.Constants.Constants;
namespace DeskMarket.Models.Product
{
    public class ProductAddViewModel
    {
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
        public IList<Category> Categories { get; set; } = new List<Category>();
    }
}
