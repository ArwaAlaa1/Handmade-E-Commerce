using Azure.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Cart;
using ECommerce.Core.Services.Contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;
        public CartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<Cart> GetCartAsync(string cartId)
        {
           var CustomerCart=await _database.StringGetAsync(cartId);
           return CustomerCart.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart>(CustomerCart);
        }

        public async Task<Cart> AddCartAsync(Cart cart)
        {
           var customercart=await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart),TimeSpan.FromDays(2));
            if (customercart is false)
            {
                return null;
            }
            return await GetCartAsync(cart.Id);
        }

        public async Task<bool> DeleteCartAsync(string cartId)
        {
            return await _database.KeyDeleteAsync(cartId);
               
        }
        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                await DeleteCartAsync($"cart:{cart.Id}");
                return null;
            }

            var _cart = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(2));
            if (_cart)
            {
                return await GetCartAsync(cart.Id);
            }
            return null;
        }


    }
}
