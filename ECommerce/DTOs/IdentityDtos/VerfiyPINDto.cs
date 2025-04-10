using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class VerfiyPINDto
    {

        [Required]
        [Range(1,999999, ErrorMessage = "Pin Code Must be 6 digits")]
        public int pin { get; set; }
    }
}
