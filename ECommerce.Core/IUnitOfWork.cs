<<<<<<< HEAD
﻿using ECommerce.Core.Repository.Contract;
=======
﻿using ECommerce.Core.Models;
>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IUnitOfWork : IDisposable
    {
<<<<<<< HEAD
        IGenericRepository<T> Repository<T>() where T : class;
        //IProductRepository ProductRepository { get; }
        //ICategoryRepository CategoryRepository { get; }
        void Save();
=======
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveAsync();
>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
    }
}
