using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Models.Order;

namespace ECommerce.Core.Repository.Contract
{
    public interface IShippingCostRepository : IGenericRepository<ShippingCost>
    {
        Task<ShippingCost?> GetShippingCostByIdAsync(int id);
    }
}
