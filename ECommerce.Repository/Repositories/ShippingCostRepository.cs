using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository.Repositories
{
    public class ShippingCostRepository : GenericRepository<ShippingCost>, IShippingCostRepository
    {
        private readonly ECommerceDbContext context;

        public ShippingCostRepository(ECommerceDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<ShippingCost?> GetShippingCostByIdAsync(int id)
        {
            return await context.ShippingCosts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
