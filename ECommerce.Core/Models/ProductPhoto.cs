using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class ProductPhoto : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public string PhotoLink { get; set; }

        // Foreign key to the related product
        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}
