using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{

    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
