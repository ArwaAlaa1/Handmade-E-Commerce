using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Review:BaseEntity
    {
        //[Key]
        //public int Id { get; set; }
        [MaxLength(250)]
        public string ReviewContent { get; set; }
        [Required]
        [RegularExpression("^(10|[1-9])$",ErrorMessage = "Please enter a number between 1 and 10. ")]
        public int Rating { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product? product { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? user { get; set; }
    }
}
