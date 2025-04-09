using ECommerce.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        void Add(T obj);

        void Update(T obj);
    }
}