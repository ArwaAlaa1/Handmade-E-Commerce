using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T obj);

        void Update(T obj);
    }
}