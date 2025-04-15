using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class Order:BaseEntity
    {
        public string CustomerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ShippingCost shippingCost { get; set; }
        public Address ShippingAddress { get; set; }
        public int? ShippingCostId { get; set; }
        
        public decimal SubTotal { get; set; }
        public decimal GetTotal()
             => SubTotal + shippingCost.Cost;
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public string PaymentId { get; set; } = "";
    }
}
