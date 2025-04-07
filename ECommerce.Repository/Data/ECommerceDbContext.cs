using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DashBoard.Data
{
    public class ECommerceDbContext:IdentityDbContext
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            :base(options)
        {
            
        }
    }
}
