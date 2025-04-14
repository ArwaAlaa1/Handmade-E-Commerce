using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Cart
{
    public class Cart
    {
        public string Id { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
