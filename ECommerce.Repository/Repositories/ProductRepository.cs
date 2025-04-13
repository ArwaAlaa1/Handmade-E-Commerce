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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ECommerceDbContext _context;

        public ProductRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithFilters(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice)
        {
            var query = _context.Products.Where(p => p.IsDeleted == false).Include(c => c.Category).Include(i => i.ProductPhotos).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Cost >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Cost <= maxPrice.Value);

            return await query.OrderBy(Product => Product.Id)
                .Where(p => p.Category.IsDeleted == false)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithOffer(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice)
        {
            var query = _context.Products.Where(p => p.IsDeleted == false).Include(c => c.Category).Include(d => d.Sales).Include(i => i.ProductPhotos).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Cost >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Cost <= maxPrice.Value);

            return await query.OrderBy(Product => Product.Id)
                .Where(p => p.Category.IsDeleted == false)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIDWithOffer(int id)
        {
            var product = await _context.Products.Include(c => c.Category).Include(s => s.Sales).Include(i => i.ProductPhotos).FirstOrDefaultAsync( p => p.Id == id);

            return product;
        }

    }
}
