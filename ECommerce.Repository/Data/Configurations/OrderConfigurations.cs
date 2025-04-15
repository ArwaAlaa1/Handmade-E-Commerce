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
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
      
         
            public void Configure(EntityTypeBuilder<Order> builder)
            {
                builder.OwnsOne(o => o.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());
                builder.Property(o => o.Status).HasConversion(
                    OStatus => OStatus.ToString(), //store
                    OStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus)//retrive
                    );
               

                builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");

            }
        
    
    }
}
