using ECommerce.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace ECommerce.DashBoard.Data
{
    public class ECommerceDbContext : IdentityDbContext<AppUser>
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>()
    .HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId)
    .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
         
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        public DbSet<Sale> Sales { get; set; }


    }
}
