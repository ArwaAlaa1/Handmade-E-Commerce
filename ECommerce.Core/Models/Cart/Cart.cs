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
        
        public Cart(string cartId)
        {
            Id = cartId;
            CartItems = new List<CartItem>();
        }

        public string Id { get; set; } // Key
        public string PaymentId { get; set; }  //PaymentIntent
        public string ClientSecret { get; set; } //ClientSecret
        public List<CartItem> CartItems { get; set; } = new List<CartItem>(); //Value
    }
}
