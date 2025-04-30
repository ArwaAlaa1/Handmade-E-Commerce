using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DTOs.ReviewDtos;
using ECommerce.Repository.Repositories;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetReview/{ReviewId}")]
        public async Task<IActionResult> GetReview(int ReviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(ReviewId);
            if (review == null)
                return NotFound("Review not found.");
            return Ok(review);
        }

        [Authorize(Roles =SD.CustomerRole)]
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDTO review)
        {
            try
            {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var rev = await _unitOfWork.Reviews.AddReview(review.ProductId,userId,review.ReviewContent,review.Rating);
           
            return Ok(new { content=review.ReviewContent,rate=review.Rating });
            }
            catch 
            {
                return BadRequest("You Are not a customer.");
            }
             
        }

        [HttpGet("EditReview/{ReviewId}")]
        public async Task<IActionResult> EditReview(int ReviewId,UpdateReviewDTO newrevewdto)
        {

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var OldReview = await _unitOfWork.Reviews.GetByIdAsync(ReviewId);
            if (OldReview == null)
                return NotFound("Review not found.");
            OldReview.ModifiedDate = DateTime.Now;
            OldReview.ReviewContent = newrevewdto.ReviewContent;
            OldReview.Rating = newrevewdto.Rating;
            
              _unitOfWork.Reviews.Update(OldReview);
            
            return Ok(OldReview);
        }

        [HttpDelete("DeleteReview/{ReviewId}")]
        public async Task<IActionResult> DeleteReview(int ReviewId)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(ReviewId);
            if (review == null)
                return NotFound("Review not found.");
            _unitOfWork.Reviews.Delete(review);
            await _unitOfWork.SaveAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _unitOfWork.Reviews.GetAllAsync();
            if (reviews == null || !reviews.Any())
                return NotFound("No reviews found.");
            return Ok(reviews);
        }



        [HttpGet("GetProductReviews/{productId}")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsWithProductAsync(productId);
                                           
            if (reviews == null || !reviews.Any())
                return NotFound("No reviews found for this product.");
            return Ok(reviews);
        }

        [HttpGet("GetReview/{reviewId}/{productId}")]
        public async Task<IActionResult> GetReview(int reviewId, int productId)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithProductAsync(reviewId, productId);
            if (review == null)
                return NotFound("Review not found.");
            return Ok(review);
        }
        
        [HttpGet("GetUserReview/{userId}")]
        public async Task<IActionResult> GetReviewWithUser(string userId)
        {
            var review = await _unitOfWork.Reviews.GetReviewWithUserAsync(userId);
            if (review == null)
                return NotFound("Review not found.");
            return Ok(review);
        }

        [HttpGet("CountOfReviews/{productId}" )]
        public async Task<IActionResult> CountOfReviews(int productId)
        {
            var count = await _unitOfWork.Reviews.CountReviewsOnProductWithId(productId);
            if (count == 0)
                return NotFound("No reviews found for this product.");
            return Ok(count);
        }
    }
}
