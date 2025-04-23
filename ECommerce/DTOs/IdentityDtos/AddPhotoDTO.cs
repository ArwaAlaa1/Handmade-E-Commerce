using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class AddPhotoDTO
    {
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Photo { get; set; }
    }
}
