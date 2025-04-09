using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class AppUser:IdentityUser
    {
        public string DisplayName { get; set; }
        public string? Photo { get; set; }

        public List<Address>? Address { get; set; }
        public bool IsActive { get; set; } = true;


        public int? PasswordResetPin { get; set; } = null;

        public DateTime? ResetExpires { get; set; } = null;

    }
}
