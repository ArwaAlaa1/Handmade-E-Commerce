using ECommerce.Core.Models.Order;

namespace ECommerce.DashBoard.ViewModels
{
    public class OrderListVM
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public OrderStatus Status { get; set; }
    }
}
