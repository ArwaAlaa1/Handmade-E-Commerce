using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class Order:BaseEntity
    {
        public Order()
        {
            
        }
        public Order(string customerEmail, int shippingCostId, int shippingAddressId
            , decimal subTotal, ICollection<OrderItem> orderItems, string? paymentId)
        {
            CustomerEmail = customerEmail;
            
            ShippingCostId = shippingCostId;
            ShippingAddressId = shippingAddressId;
           
            SubTotal = subTotal;
            OrderItems = orderItems;
            PaymentId = paymentId;
        }

        public string CustomerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ShippingCost? shippingCost { get; set; }
        public Address? ShippingAddress { get; set; }
        public int? ShippingAddressId { get; set; }

        public int? ShippingCostId { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal GetTotal()
             => SubTotal + shippingCost.Cost;
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public string PaymentId { get; set; } = "";
    }
}
