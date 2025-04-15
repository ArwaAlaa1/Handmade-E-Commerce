using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repository.Repositories
{
    public class AddressRepository:GenericRepository<Address>,IAddressRepository
    {
        public AddressRepository(ECommerceDbContext dbContext): base(dbContext)
        {
            
        }
    }
}
