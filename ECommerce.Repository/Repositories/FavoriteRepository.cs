﻿using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<List<Product>> GetAllUserFavorite(string userId)
        {
            var favorites = await _db.Favorites
                                     .Where(f => f.UserId == userId)
                                     .Include(f => f.product)
                                     .ToListAsync();

            var products = favorites.Select(f => f.product).ToList();
            return products;
        }

        public async Task<bool> isFavorite(int productId, string userId)
        {
            var favroites = await _db.Favorites.Where(f => f.UserId == userId && f.ProductId == productId).Include(z => z.product).FirstOrDefaultAsync();
            if (favroites == null) return true;
            else return false;
        }



        public async Task<List<int>> GetFavoriteProductIdsAsync(string userId, List<int> productIds)
        {
            return await _db.Favorites
                .Where(f => f.UserId == userId && productIds.Contains(f.ProductId))
                .Select(f => f.ProductId)
                .ToListAsync();
        }

    }
}
