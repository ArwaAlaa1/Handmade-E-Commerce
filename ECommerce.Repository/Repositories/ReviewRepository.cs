using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class ReviewRepository:GenericRepository<Review>,IReviewRepository
    {
        private readonly ECommerceDbContext _db;
        public ReviewRepository(ECommerceDbContext db):base(db)
        {
            _db = db;
        }

        public async Task<int> CountReviewsOnProductWithId(int productId)
        {

            return await _db.Reviews.CountAsync(z=>z.ProductId==productId);
        }

        public async Task<Review> AddReview(int productid,string userid,string content,int rate)
        {
            Review rev = new Review()
            {
                ProductId = productid,
                UserId = userid,
                ReviewContent = content,
                Rating = rate
            };
           var res= await _db.Reviews.AddAsync(rev);
           var ress= await _db.SaveChangesAsync();
            return rev;
        }
        public async Task<int> SumRatingOnProductWithId(int productId)
        {
            return await _db.Reviews
                            .Where(z => z.ProductId == productId)
                            .SumAsync(x => x.Rating);
        }


        public async Task<IEnumerable<Review>> GetReviewsWithProductAsync(int productId)
        {
            return await _db.Reviews.Where(x=>x.ProductId==productId).Include(z=>z.user).ToListAsync();
        }

        public async Task<Review> GetReviewWithProductAsync(int reviewId, int productId)
        {
            return await
                _db.Reviews 
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.ProductId == productId);
        }

        public async Task<Review> GetReviewWithUserAsync(string userid)
        {
            return await _db.Reviews
                .Include(r => r.user)
                .FirstOrDefaultAsync(r => r.UserId == userid);
        }

        public async Task<Dictionary<int, int>> GetReviewsCountForProductsAsync(List<int> productIds)
        {
            return await _db.Reviews
                .Where(r => productIds.Contains(r.ProductId))
                .GroupBy(r => r.ProductId)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Sum(r => r.Rating)
                );
        }

    }
}
