using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Cart;
using ECommerce.Core.Services.Contract;
using ECommerce.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;
using static StackExchange.Redis.Role;

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
            //if (cartId!=null && !cartId.StartsWith("cart") && userId != null)
            //{

            //    Cart cart1 = new Cart();
            //    if (userId != null)
            //    {
            //       var cartguest = await _cartRepository.GetCartAsync($"{cartId}");
                   
                       
            //        cart1.Id = $"cart:{userId}";
            //        cart1.CartItems = cartguest.CartItems;
            //        var Createdfromguest = await _cartRepository.AddCartAsync(cart1);

            //        return Ok(Createdfromguest);

            //    }
            //}

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
            
            //var mappedcustomerbasket = mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreateOrUpdateBasket = await _cartRepository.AddCartAsync(cart);
            //if (CreateOrUpdateBasket is null) return BadRequest(new ApiResponse(400));
            return Ok(CreateOrUpdateBasket);

            //var userId = User.Identity.IsAuthenticated
            //    ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            //    : null;

            //if (userId != null)
            //{

            //    var userCart = await _cartRepository.GetCartAsync($"cart:{userId}");
            //    if (userCart != null)
            //    {
            //        userCart.CartItems.AddRange(cart.CartItems);
            //        await _cartRepository.AddCartAsync(userCart);
            //        await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
            //        return Ok(userCart);
            //    }
            //    else
            //    {
            //        var newCart = new Cart
            //        {
            //            Id = $"cart:{userId}",
            //            CartItems = cart.CartItems
            //        };
            //        await _cartRepository.AddCartAsync(newCart);
            //        await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
            //        return Ok(newCart);
            //    }

            //}
            //else
            //{
            //    var existingCart = await _cartRepository.GetCartAsync($"cart:{cart.Id}");
            //    if (existingCart == null)
            //    {
            //        var newCart = new Cart
            //        {
            //            Id = $"cart:{cart.Id}",
            //            CartItems = cart.CartItems
            //        };
            //        await _cartRepository.AddCartAsync(newCart);
            //        return Ok(newCart);

            //    }
            //    else
            //    {
            //        existingCart.CartItems = cart.CartItems;

            //        await _cartRepository.AddCartAsync(existingCart);
            //        return Ok(existingCart);

            //    }
            //}




        }

        //[HttpPost("UpdateCart")]
        //public async Task<IActionResult> UpdateCart([FromBody] Cart cart)
        //{
        //    var userId = User.Identity.IsAuthenticated
        //       ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //       : null;
        //    var existingCart = new Cart();
        //    if (userId != null)
        //    {
        //         existingCart = await _cartRepository.GetCartAsync($"cart:{userId}");
        //    }
        //    else
        //    {
        //        existingCart = await _cartRepository.GetCartAsync($"cart:{cart.Id}");

        //    }
        //    if (existingCart != null)
        //    {

        //        await _cartRepository.AddCartAsync(cart);
        //        if (cart.CartItems.Count()==0)
        //        {
        //            await _cartRepository.DeleteCartAsync($"cart:{cart.Id}");
        //        }
        //            return Ok(new { message="Cart Updated Successfully"});

        //    }
        //    return Ok(existingCart);

        //}

        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart([FromBody] Cart cart)
        {
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;

                var result = await _cartRepository.UpdateCartAsync(cart);
            return Ok(result);
        }
        [HttpDelete("DeleteCartAsync")]
        public async Task<IActionResult> DeleteCartAsync( [FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("The cartId field is required.");
                }

                // Delete the cart from Redis (or your database)
                bool deleted = await _cartRepository.DeleteCartAsync(id);
                if (!deleted)
                {
                    return NotFound("Cart not found or could not be deleted.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
