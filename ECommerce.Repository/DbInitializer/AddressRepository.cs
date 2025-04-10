using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.DbInitializer
{
    internal class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        private readonly ECommerceDbContext _context;

        public AddressRepository(ECommerceDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
