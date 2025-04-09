using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _db;
        private Hashtable _repositories;
        public UnitOfWork(ECommerceDbContext db)
        {
            _db = db;
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

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
