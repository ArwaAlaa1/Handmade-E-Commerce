using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.Core.Services.Contract;
using ECommerce.DashBoard.Data;
using ECommerce.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ECommerce.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _db;
        //private readonly IConnectionMultiplexer redis;
        private Hashtable _repositories;

        public IReviewRepository Reviews { get; private set; }
        public IFavoriteRepository Favorites { get; private set; }

        public IProductRepository Products { get; private set; }
        //public ICustomerBasketRepository CustomerBaskets { get;}
        //public ICartRepository cartRepository { get; }
        //public IShippingCostRepository ShippingCosts { get; }

        public UnitOfWork(ECommerceDbContext db,
                   IReviewRepository reviewRepository,
                   IFavoriteRepository favoriteRepository
                   /*,IConnectionMultiplexer redis*/, IProductRepository productRepository)

        {
            _db = db;
            Reviews = reviewRepository;
            Favorites = favoriteRepository;

            Products = productRepository;

            //this.redis = redis;
            //CustomerBaskets = new CustomerBasketRepository(redis);
            //cartRepository = new CartRepository(redis);
            //ShippingCosts = new ShippingCostRepository(_db);

            _repositories = new Hashtable();
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;
            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<T>(_db);
                _repositories.Add(key, repository);
            }
            return _repositories[key] as IGenericRepository<T>;
        }

        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();

        }
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
