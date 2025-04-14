using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Cart
{
    public class Cart
    {
        public Cart()
        {
            
        }
        private string cartId;

        public Cart(string cartId)
        {
            this.cartId = cartId;
            CartItems = new List<CartItem>();
        }

        public string Id { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
