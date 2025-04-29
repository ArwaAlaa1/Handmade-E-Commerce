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
            var isfav = await _unitOfWork.Favorites.isFavorite(productId, userId);
            if (isfav)
            {
                var product = await _unitOfWork.Favorites.AddFavoriteproducttoUser(productId, userId);
                return Ok("Add to Product Succesflly");
            }
            else return BadRequest("this Product already in Fav List");
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

            //var isfav = await _unitOfWork.Favorites.isFavorite(productId, userId);
            //if (!isfav)
            //{
            //    var product = await _unitOfWork.Favorites.AddFavoriteproducttoUser(productId, userId);
            //    return Ok(product);
            //}
            //else return BadRequest("this product Not in Fav List");

            bool resut = await _unitOfWork.Favorites.RemoveFavoriteproducttoUser(productId, userId);
            if (resut)
            {
                return Ok("Deleted Succesfully");
            }else
            {
                return NotFound("This Product not found in favorites.");
            }
        }

        [HttpGet("GetUserFavorite/{userid}")]
        public async Task<ActionResult> GetUserFavorites(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID cannot be null or empty.");
<<<<<<< HEAD

            var favorites = await _unitOfWork.Favorites.GetAllUserFavorite(userId);

            if (favorites == null || favorites.Count == 0)
                return NotFound("No favorites found for this user.");

=======
           
            var favorites = _unitOfWork.Favorites.GetAllUserFavorite(userId);
            //var Favoritesproducts = favorites.Result;
            if (favorites == null || !favorites.IsCompleted )
                return NotFound("No favorites found for this user.");
>>>>>>> 3fe2424c398972ee1969bf6cf3e067d0c11d7b2e
            return Ok(favorites);
        }


    }
}
