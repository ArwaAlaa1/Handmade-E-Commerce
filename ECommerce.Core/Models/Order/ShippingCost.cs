using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class ShippingCost:BaseEntity
    {
      
        //public ShippingCost(string _Name,  string deliveryTime, decimal cost)
        //{
        //    Name = _Name;
           
        //    DeliveryTime = deliveryTime;
        //    Cost = cost;
        //}

        public string Name { get; set; }
        public string DeliveryTime { get; set; }
        public decimal Cost { get; set; }

    }
}
