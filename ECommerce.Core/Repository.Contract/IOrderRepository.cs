using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Repository.Contract
{
    public interface IOrderRepository:IGenericRepository<Order>
    {
        public Task<IEnumerable<Order>> GetUserOrdersAsync(string Email);
        public Task<Order> GetOrderForUserAsync(int OrderId);
    }
}
