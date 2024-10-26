﻿using Microsoft.AspNetCore.Authentication;

namespace DeskMarket.Models.Product
{
    public class ProductDetailsModelView
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string AddedOn { get; set; }
        public string Seller { get; set; }
        public bool HasBought { get; set; }

    }
}
