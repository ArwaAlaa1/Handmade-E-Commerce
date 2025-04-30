using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Repository.Contract
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        
        Task<List<Review>> GetReviewsWithProductAsync(int productId);
        Task<int> CountReviewsOnProductWithId(int productId);
        Task<int> SumRatingOnProductWithId(int productId);
        Task<Review> GetReviewWithProductAsync(int reviewId,int productId);
        Task<Review> AddReview(int productid, string userid, string content, int rate);
        Task<Review> GetReviewWithUserAsync(string userid);

        Task<Dictionary<int, int>> GetReviewsCountForProductsAsync(List<int> productIds);

    }
}
