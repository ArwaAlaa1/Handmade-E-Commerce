﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerce.Core.Models
{
    public class Product:BaseEntity
    {
        
        public string Name { get; set; }

        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string? AdditionalDetails { get; set; }
        public int Stock { get; set; }

        public decimal AdminProfitPercentage { get; set; }
        public string? SellerId { get; set;}
        public AppUser? Seller { get; set; }


        // Foreign key
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }


        public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

        public ICollection<Sale> Sales { get; set; }

        public ICollection<ProductSize> ProductSizes { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }


        [NotMapped]
        public bool IsOnSale => Sales?.Any(s =>
            s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today) == true;

        [NotMapped]
        public decimal DiscountedPrice
        {
            get
            {
                var currentSale = Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);
                
                if (currentSale != null)
                {
                    var discount = SellingPrice * currentSale.Percent / 100;
                    return SellingPrice - discount;
                }

                return SellingPrice;
            }
           
        }

        [NotMapped]
        public decimal SellingPrice
        {
            get
            {
                decimal adminProfit = Cost * AdminProfitPercentage / 100;
                return Cost + adminProfit;
            }
        }

        [NotMapped]
        public int? SaleId => Sales?.FirstOrDefault(s =>
            s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today)?.Id;

        [NotMapped]
        public decimal FinalPrice => IsOnSale ? DiscountedPrice : SellingPrice; //==>Calculate the final price based on whether the product is on sale or not
    }
}
