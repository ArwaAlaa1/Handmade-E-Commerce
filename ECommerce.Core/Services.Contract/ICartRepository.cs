using ECommerce.Core.Models.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services.Contract
{
    public interface ICartRepository
    {

        Task<Cart> GetCartAsync(string id);
        Task<Cart> AddCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string id);

    }
}
