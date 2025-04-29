
﻿using ECommerce.Core.Repository.Contract;
﻿using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Services.Contract;

namespace ECommerce.Core
{
    public interface IUnitOfWork : IDisposable
    {

        IReviewRepository Reviews { get; }
        IFavoriteRepository Favorites { get; }

        IProductRepository Products { get; }


        //ICustomerBasketRepository CustomerBaskets { get; }
        //ICartRepository cartRepository { get; }
        //public IShippingCostRepository ShippingCosts { get; }


        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveAsync();

    }
}
