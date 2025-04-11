using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old Password Is Required !")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New Password Is Required !")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Connfirmed Password Is Required !")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
    }
}
