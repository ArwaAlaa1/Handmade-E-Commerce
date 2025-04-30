using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string CustomerEmail, string CartId, int shippingCostId, int ShippingAddress, string paymentId);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string Email);
        Task<Order> GetOrderForUserAsync(int orderid);
        Task<Order> CancelOrder(int orderid);
        Task<OrderItem> CancelItemOrder(int orderItemId);
    }
}
