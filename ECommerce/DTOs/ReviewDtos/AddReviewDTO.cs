using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.ReviewDtos
{
    public class AddReviewDTO
    {
        [Required]
        [StringLength(400)] 
        public string ReviewContent { get; set; }
        [Required]
        [RegularExpression("^(10|[1-9])$", ErrorMessage = "Please enter a number between 1 and 10. ")]
        public int Rating { get; set; }

        public int ProductId { get; set; }
        


    }
}
