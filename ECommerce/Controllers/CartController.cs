using ECommerce.Core.Models.Cart;
using ECommerce.Core.Services.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCart(string cartId)
        {
            var cart = await _cartRepository.GetCartAsync(cartId);
            if (cart == null)
            {
                return Ok(new Cart(cartId));
            }
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] Cart cart)
        {
            
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            var existingCart = await _cartRepository.GetCartAsync($"cart:{cart.Id}");


            if (existingCart == null)
            {
                if (userId != null)
                {
                  

                    var newCart = new Cart
                    {
                        Id = $"cart:{userId}",
                        CartItems = cart.CartItems
                    };
                    await _cartRepository.AddCartAsync(newCart);
                    return Ok(newCart);

                }
                else
                {
                    var newCart = new Cart
                    {
                        Id = $"cart:{cart.Id}",
                        CartItems = cart.CartItems
                    };
                    await _cartRepository.AddCartAsync(newCart);
                    return Ok(newCart);
                }

            }
            else
            {
                if (userId != null)
                {
                    existingCart.Id = $"cart:{userId}";
                    existingCart.CartItems = cart.CartItems;
                    await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
                    

                    await _cartRepository.AddCartAsync(existingCart);
                    return Ok(existingCart);

                }
                else
                {
                   
                    existingCart.CartItems = cart.CartItems;

                    await _cartRepository.AddCartAsync(existingCart);
                    return Ok(existingCart);

                }
             
            }


        }
        [HttpDelete]
        public async Task DeleteCart(string id)
        {
             await _cartRepository.DeleteCartAsync(id);
        }


    }
}
