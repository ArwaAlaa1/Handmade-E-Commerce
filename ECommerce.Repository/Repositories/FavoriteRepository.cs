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
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        private readonly ECommerceDbContext _db;
       
        public FavoriteRepository(ECommerceDbContext db) : base(db)
        {
            _db = db;
           
        }

        public async Task<Product> AddFavoriteproducttoUser(int productid, string userid)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == productid);
            var user=_db.Users.FirstOrDefault(p => p.Id == userid);
            
            if (product == null || user == null)
            {
                return new Product() { };
            }

                var favorite = new Favorite
                {
                    ProductId = product.Id,
                    UserId = user.Id
                };
            _db.Favorites.Add(favorite);
               await _db.SaveChangesAsync();
               return product;

           
        }
        

        public async Task<bool> RemoveFavoriteproducttoUser(int productid, string userid)
        {
            var favorite= await _db.Favorites.FirstOrDefaultAsync(f => f.ProductId == productid && f.UserId == userid);
            if (favorite != null)
            {

                _db.Favorites.Remove(favorite);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public Task<List<Product>> GetAllUserFavorite(string userId)
        {
            var favroites= _db.Favorites.Where(f => f.UserId == userId).Include(z=>z.product).ToListAsync();
            
            var products =  _db.Products.Where(p => p.Id == favroites.Result.FirstOrDefault().ProductId).ToListAsync();
             return  products;
        }
    }
}
