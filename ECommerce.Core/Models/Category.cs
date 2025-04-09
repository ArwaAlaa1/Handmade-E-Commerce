using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
<<<<<<< HEAD
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation
        public ICollection<Product> Products { get; set; }
=======
    public class Category:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }

>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
    }
}
