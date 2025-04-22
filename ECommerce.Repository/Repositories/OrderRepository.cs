using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ECommerceDbContext context) : base(context)
        {
        }

     

        public async Task<Order> GetOrderForUserAsync(int OrderId)
        {
            var order = await _db.Set<Order>()
     .Where(o => o.Id == OrderId)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Product)
             .ThenInclude(p => p.ProductPhotos)
     .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Product)
             .ThenInclude(p => p.Seller)
     .Include(o => o.shippingCost)
     .Include(o => o.ShippingAddress)
      .AsNoTracking()
     .FirstAsync();

            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string Email)
        {
            var order = await _db.Set<Order>().Where(O => O.CustomerEmail == Email && O.IsDeleted == false).Include(o => o.shippingCost).Include(o => o.OrderItems).ToListAsync();
            return order;
        }


        public Task<OrderItem?> GetItemInOrderAsync(int OrderItemId)
        {
           var orderItem = _db.Set<OrderItem>().Where(o => o.Id == OrderItemId).FirstOrDefaultAsync();
            return orderItem;
        }
    }
    
}
