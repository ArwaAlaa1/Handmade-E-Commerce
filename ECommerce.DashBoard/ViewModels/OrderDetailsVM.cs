﻿using ECommerce.Core.Models.Order;

namespace ECommerce.DashBoard.ViewModels
{
    public class OrderDetailsVM
    {
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public string Address { get; set; }
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
        public string? TraderName { get; set; }
        public ItemStatus OrderItemStatus { get; set; }
        public string? CustomizeInfo { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
    }
}
