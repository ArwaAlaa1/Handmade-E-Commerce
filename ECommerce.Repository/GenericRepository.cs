using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository
{

  
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity

    {
        private readonly ECommerceDbContext _db;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ECommerceDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }
      
       

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

      
        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await _dbSet.Where(t => t.IsDeleted == false).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

     

    }
}