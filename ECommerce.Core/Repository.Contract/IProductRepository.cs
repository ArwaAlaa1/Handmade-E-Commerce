using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Repository.Contract
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();

        Task<IEnumerable<Product>> GetProductsWithFilters(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice);

        Task<IEnumerable<Product>> GetProductsWithOffer(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice);

        Task<Product> GetProductByIDWithOffer(int id);

    }
}
