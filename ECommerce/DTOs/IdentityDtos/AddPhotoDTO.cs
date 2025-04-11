using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class AddPhotoDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
    }
}
