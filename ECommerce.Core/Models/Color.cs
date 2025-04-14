using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Color : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Optional: Who added this color (for future Trader use)
        public int? AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser? Trader { get; set; }

        public ICollection<ProductColor> ProductColors { get; set; }

    }
}
