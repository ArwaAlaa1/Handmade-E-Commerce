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
<<<<<<< HEAD
    public class GenericRepository<T> : IGenericRepository<T> where T : class
=======
    internal class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
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

<<<<<<< HEAD

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        //public void Update(T entity)
        //{
        //    _dbSet.Update(entity);
        //}

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
=======
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
      
        
>>>>>>> 0804e9add3b9992e97b915c34bf6f24661df96d5
    }
}