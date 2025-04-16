using ECommerce.Core.Models.Order;

namespace ECommerce.DashBoard.ViewModels
{
    public class OrderDetailsVM
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal ShippingCost { get; set; }
        public List<OrderItemVM> OrderItems { get; set; }
    }
    public class OrderItemVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal ProductCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
