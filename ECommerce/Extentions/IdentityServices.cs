using ECommerce.Core.Models;
using ECommerce.Core.Services.Contract;
using ECommerce.DashBoard.Data;
using ECommerce.Services;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Extentions
{
    public static class IdentityServices
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<AppUser, IdentityRole>()
               .AddEntityFrameworkStores<ECommerceDbContext>()
               .AddDefaultTokenProviders();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
