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
        protected readonly ECommerceDbContext _db;
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


        public async Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>> filter = null,
    string includeProperties = "")
        {
            IQueryable<T> query = _dbSet.Where(t => !t.IsDeleted);

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.AsNoTracking().ToListAsync();
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            var result = await _dbSet.FindAsync(id);
            return result;
        }

        public async Task<T?> GetByIdWithIncludeAsync(int id, string includeProperties)
        {
            IQueryable<T> query = _dbSet;

            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty.Trim());
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }



        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

    }
}