using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Models.Cart;
using ECommerce.Core.Models.Order;

namespace ECommerce.Core.Services.Contract
{
    public interface IPaymentService
    {
        //Task<Order>CreateOrUpdatePaymentAsync(string customerEmail, string cartId, int shippingCostId, int shippingAddressId);
        Task<Cart> CreateOrUpdatePaymentAsync(string CardId, int? shippingCostId);
    }
}
