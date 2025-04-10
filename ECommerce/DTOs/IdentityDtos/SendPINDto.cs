using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class SendPINDto
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
