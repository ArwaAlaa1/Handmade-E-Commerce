using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.ReviewDtos
{
    public class UpdateReviewDTO
    {
        [Required]
        public string ReviewContent { get; set; }
        [Required]
        [RegularExpression("^(10|[1-9])$", ErrorMessage = "Please enter a number between 1 and 10. ")]
        public int Rating { get; set; }

    }
}
