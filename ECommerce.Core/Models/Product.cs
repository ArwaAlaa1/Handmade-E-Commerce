using System;
using System.Collections.Generic;
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

        // Foreign key
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();
    }
}
