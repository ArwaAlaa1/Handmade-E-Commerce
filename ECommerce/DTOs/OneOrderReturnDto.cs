using ECommerce.DTOs.OrderDtos;

namespace ECommerce.DTOs
{
    public class OneOrderReturnDto
    {
        public int OrderId { get; set; }

        public string OrderDate { get; set; } 
        public string Status { get; set; }
        public int ItemsCount { get; set; }
        public decimal  SubTotal { get; set; }
        public int ShippingCost { get; set; }
        public decimal Total { get; set; }
        public ICollection<OneItemInOrderReturnDto> OrderItems { get; set; }
        public AddressReturnDto ShippingAddress { get; set; }
       
        public string PaymentId { get; set; } = "";
    }
}
