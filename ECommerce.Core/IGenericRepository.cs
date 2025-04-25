using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync(
    Expression<Func<T, bool>> filter = null,
    string includeProperties = "");

        Task<T?> GetByIdAsync(int id);

        public Task<T?> GetByIdWithIncludeAsync(int id, string includeProperties);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);

        IQueryable<T> GetQueryable(Func<IQueryable<T>, IQueryable<T>> include = null);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
