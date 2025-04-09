using ECommerce.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DashBoard.Data
{
    public class ECommerceDbContext : IdentityDbContext<AppUser>
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options)
        { }

<<<<<<< HEAD
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


=======
        public DbSet<Category> Categories { get; set; }
>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
    }
}
