using ECommerce.Core.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Data.Configurations
{
   public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
            //builder.Property(o => o.OrderItemStatus)
            //       .HasConversion<string>();

            builder.Property(o => o.OrderItemStatus).HasConversion(
                 OStatus => OStatus.ToString(),
                 OStatus => (ItemStatus)Enum.Parse(typeof(ItemStatus), OStatus)
             );
            //builder.Property(o => o.OrderItemStatus).HasConversion<string>();


        }
    }
}
