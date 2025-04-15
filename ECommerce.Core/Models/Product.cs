using System;
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
        //public string Photo { get; set;}
        public int? SallerId { get; set;}
        public AppUser Seller { get; set; }

        // Foreign key
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();

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
                    var discount = Cost * currentSale.Percent / 100;
                    return Cost - discount;
                }

                return Cost;
            }
        }

        [NotMapped]
        public int? SaleId => Sales?.FirstOrDefault(s =>
            s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today)?.Id;

    }
}
