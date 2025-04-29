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
    }
}
