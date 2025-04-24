using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Repository.Repositories;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public FavoriteController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        [Authorize(Roles =SD.CustomerRole)]
        [HttpPost("AddFavoriteToUser/{productId}")]
        public async Task<ActionResult> AddFavorite(int productId)
        {
            var userId = _userManager.GetUserId(User);

            if(User.Identity.IsAuthenticated == false)
            {
                return Unauthorized("User is not authenticated.");
            }
            var product=await _unitOfWork.Favorites.AddFavoriteproducttoUser(productId, userId);
            return Ok(product);
        }

        [Authorize(Roles = SD.CustomerRole)]
        [HttpDelete("DeleteFavorite/{productId}")]
        public async Task<ActionResult> DeleteFavorite(int productId) 
        {
            var userId =  _userManager.GetUserId(User);
            if (User.Identity.IsAuthenticated == false)
            {
                return Unauthorized("User is not authenticated.");
            }
            if (string.IsNullOrEmpty(userId) || productId <= 0)
                return BadRequest("User ID or Product ID cannot be null or empty.");
            bool resut= await _unitOfWork.Favorites.RemoveFavoriteproducttoUser(productId, userId);
            if (resut)
            {
                return Ok("Deleted Succesfully");
            }else
            {
                return NotFound("Product not found in favorites.");
            }
        }

        [HttpGet("GetUserFavorite/{userid}")]
        public ActionResult GetUserFavorites(string userId) 
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID cannot be null or empty.");
           
            var favorites = _unitOfWork.Favorites.GetAllUserFavorite(userId);
            var Favoritesproducts = favorites.Result;
            if (favorites == null || !favorites.IsCompleted )
                return NotFound("No favorites found for this user.");
            return Ok(Favoritesproducts);
        }

    }
}
