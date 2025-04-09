using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository
{
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ECommerceDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ECommerceDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
      
        public void Add(T entity)
        {
            _dbSet.Add(entity);

        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var result = await _dbSet.Where(t => t.IsDeleted == false).ToListAsync();
            return result;
        }

        public async Task<T?> GetById(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
      
        
    }
}