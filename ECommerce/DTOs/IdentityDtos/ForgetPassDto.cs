using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class ForgetPassDto
    {

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
