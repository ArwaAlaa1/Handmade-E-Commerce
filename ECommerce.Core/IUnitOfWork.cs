
﻿using ECommerce.Core.Repository.Contract;
﻿using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IUnitOfWork : IDisposable
    {

        IReviewRepository Reviews { get; }
        IFavoriteRepository Favorites { get; }

        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveAsync();

    }
}
