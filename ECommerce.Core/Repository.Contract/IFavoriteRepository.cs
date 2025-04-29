using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Repository.Contract
{
    public interface IFavoriteRepository:IGenericRepository<Favorite>
    {

        Task<Product> AddFavoriteproducttoUser(int productid,string userid);
        Task<List<Product>> GetAllUserFavorite(string userId);
        Task<bool> RemoveFavoriteproducttoUser(int productid, string userid);
        Task<bool> isFavorite(int productId, string userId);
        Task<List<int>> GetFavoriteProductIdsAsync(string userId, List<int> productIds);

    }
}
