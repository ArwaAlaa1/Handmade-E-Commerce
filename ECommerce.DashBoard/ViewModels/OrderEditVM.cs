using ECommerce.Core.Models.Order;

namespace ECommerce.DashBoard.ViewModels
{
    public class OrderEditVM
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }

        public decimal? ShippingCost { get; set; }
    }
}
