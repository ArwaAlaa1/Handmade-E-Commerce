using ECommerce.Core.Models.Order;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.ViewModels
{
    public class OrderItemEditVM
    {
        public int OrderItemId { get; set; }
        public string? ProductName { get; set; }
        public ItemStatus? OrderItemStatus { get; set; }
        public IEnumerable<SelectListItem>? StatusOptions { get; set; }
    }
}
