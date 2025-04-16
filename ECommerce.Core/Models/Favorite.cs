using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Favorite:BaseEntity
    {
        
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product product { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser user { get; set; }



    }
}
