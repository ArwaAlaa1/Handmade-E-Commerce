using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "User Name or Email is required")]
        public string EmailOrUserName { get; set; }

       

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
