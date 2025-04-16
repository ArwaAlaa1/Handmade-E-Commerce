using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace ECommerce.DashBoard.Data
{
    public class ECommerceDbContext : IdentityDbContext<AppUser>
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
            : base(options)
        { }

      
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        public DbSet<Sale> Sales { get; set; }

        //public DbSet<Color> Colors { get; set; }
        //public DbSet<Size> Sizes { get; set; }

        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShippingCost> ShippingCosts{ get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sale>()
                  .HasOne(s => s.Product)
                  .WithMany(p => p.Sales)
                  .HasForeignKey(s => s.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);


    //        modelBuilder.Entity<ProductSize>()
    //.HasKey(ps => new { ps.ProductId, ps.SizeId });

            modelBuilder.Entity<ProductSize>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSizes)
                .HasForeignKey(ps => ps.ProductId);

            //modelBuilder.Entity<ProductSize>()
            //    .HasOne(ps => ps.Size)
            //    .WithMany(s => s.ProductSizes)
            //    .HasForeignKey(ps => ps.SizeId);


        //    modelBuilder.Entity<ProductColor>()
        //.HasKey(pc => new { pc.ProductId, pc.ColorId });

            modelBuilder.Entity<ProductColor>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(pc => pc.ProductId);

            //modelBuilder.Entity<ProductColor>()
            //    .HasOne(pc => pc.Color)
            //    .WithMany(c => c.ProductColors)
            //    .HasForeignKey(pc => pc.ColorId);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceDbContext).Assembly);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
