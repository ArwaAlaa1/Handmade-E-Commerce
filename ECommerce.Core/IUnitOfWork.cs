
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

        IGenericRepository<T> Repository<T>() where T : class;
        //IProductRepository ProductRepository { get; }
        //ICategoryRepository CategoryRepository { get; }
        void Save();
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveAsync();

    }
}
