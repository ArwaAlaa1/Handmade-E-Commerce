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

        [HttpGet]
        public async Task<IActionResult> GetCart(string? cartId)
        {
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
            var cart =new Cart();
            if (userId !=null)
            {
                cart = await _cartRepository.GetCartAsync($"cart:{userId}");
                if (cart == null)
                {
                    return Ok(new Cart($"cart:{userId}"));
                }
            }
            else
            {
                cart = await _cartRepository.GetCartAsync(cartId);
                if (cart == null)
                {
                    return Ok(new Cart(cartId));
                }
            }
           
            return Ok(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] Cart cart)
        {
            
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

            if (userId != null)
            {
                
                var userCart = await _cartRepository.GetCartAsync($"cart:{userId}");
                if (userCart != null)
                {
                    userCart.CartItems.AddRange(cart.CartItems);
                    await _cartRepository.AddCartAsync(userCart);
                    await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
                    return Ok(userCart);
                }
                else
                {
                    var newCart = new Cart
                    {
                        Id = $"cart:{userId}",
                        CartItems = cart.CartItems
                    };
                    await _cartRepository.AddCartAsync(newCart);
                    await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
                    return Ok(newCart);
                }
                
            }
            else
            {
                var existingCart = await _cartRepository.GetCartAsync($"cart:{cart.Id}");
                if (existingCart == null)
                {
                    var newCart = new Cart
                    {
                        Id = $"cart:{cart.Id}",
                        CartItems = cart.CartItems
                    };
                    await _cartRepository.AddCartAsync(newCart);
                    return Ok(newCart);

                }
                else
                {
                    existingCart.CartItems = cart.CartItems;

                    await _cartRepository.AddCartAsync(existingCart);
                    return Ok(existingCart);

                }
            }

            


        }

        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart([FromBody] Cart cart)
        {
            var userId = User.Identity.IsAuthenticated
               ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               : null;
            var existingCart = new Cart();
            if (userId != null)
            {
                 existingCart = await _cartRepository.GetCartAsync($"cart:{userId}");
            }
            else
            {
                existingCart = await _cartRepository.GetCartAsync($"cart:{cart.Id}");

            }
            if (existingCart != null)
            {
        
                await _cartRepository.AddCartAsync(cart);
                if (cart.CartItems.Count()==0)
                {
                    await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
                }
                    return Ok(new { message="Cart Updated Successfully"});
            
            }
            return Ok(existingCart);




        }
        [HttpDelete]
        public async Task DeleteCart(string id)
        {
             await _cartRepository.DeleteCartAsync(id);
        }


    }
}
