using ECommerce.Core.Models;
using ECommerce.DashBoard.Data;
using Microsoft.AspNetCore.Identity;
using ECommerce.Services.Utility;

namespace ECommerce.Repository.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<AppUser>  _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ECommerceDbContext _dbcontext;

        public DbInitializer(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ECommerceDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontext = dbContext;
        }

        public void Initializer()
        {
            try
            {
                //if (_dbcontext.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.SuplierRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();
                }

                //if (_dbcontext.Users.Any())
                {
                    _userManager.CreateAsync(new AppUser
                    {
                        Email = "admin@gmail.com",
                        UserName = "Admin",
                        EmailConfirmed = true,
                        DisplayName= "Admin",
                        IsActive = true,
                    }, "P@ssw0rd").GetAwaiter().GetResult();

                    var admin = _dbcontext.Users.FirstOrDefault(x => x.Email == "admin@gmail.com");

                    if (admin != null)
                        _userManager.AddToRoleAsync(admin, SD.AdminRole).GetAwaiter().GetResult();

                    return;
                }
            }
            catch(Exception ex)
            {
            }
        }
    }
}
