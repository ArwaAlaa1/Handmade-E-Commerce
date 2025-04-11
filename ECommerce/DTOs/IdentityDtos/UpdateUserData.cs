using ECommerce.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.IdentityDtos
{
    public class UpdateUserData
    {
        [Required]
        [MinLength(5, ErrorMessage = "Username must be at least 5 characters long")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

    }
}
