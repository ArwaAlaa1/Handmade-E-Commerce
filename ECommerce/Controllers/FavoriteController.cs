using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public FavoriteController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       

        [HttpPost("AddFavoriteToUser")]
        public ActionResult AddFavorite(string userid,int productId)
        {
            var product=_unitOfWork.Favorites.AddFavoriteproducttoUser(productId, userid);
            return Ok(product);
        }

        [HttpDelete("DeleteFavorite/{userid}/{productId}")]
        public async Task<ActionResult> DeleteFavorite(string userid,int productid) 
        {
            if (string.IsNullOrEmpty(userid) || productid <= 0)
                return BadRequest("User ID or Product ID cannot be null or empty.");
            bool resut= await _unitOfWork.Favorites.RemoveFavoriteproducttoUser(productid,userid);
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
