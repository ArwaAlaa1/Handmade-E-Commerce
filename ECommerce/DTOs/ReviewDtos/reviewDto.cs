using ECommerce.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTOs.ReviewDtos
{
    public class reviewDto
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public string ReviewContent { get; set; }
        
        [Required]
        [RegularExpression("^(10|[1-9])$", ErrorMessage = "Please enter a number between 1 and 10. ")]
        public int Rating { get; set; }

        //public AppUser user { get; set; }
    }
}
