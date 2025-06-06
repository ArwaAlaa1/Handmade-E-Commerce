﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class OrderItem:BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? CustomizeInfo { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public ItemStatus OrderItemStatus { get; set; } = ItemStatus.Pending;
        public decimal TotalPrice { get; set; }

        public OrderItem(int productId, string? customizeInfo, string? color, string? size, 
            string traderId, int quantity)
        {
            ProductId = productId;
         
            CustomizeInfo = customizeInfo;
            Color = color;
            Size = size;
           
           
            TraderId = traderId;
            Quantity = quantity;
        }

      

        public string TraderId { get; set; }
        public int Quantity { get; set; }

      
    }
}
