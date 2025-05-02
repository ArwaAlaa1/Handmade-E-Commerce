namespace ECommerce.DTOs.OrderDtos
{
    public class OneItemInOrderReturnDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string Photo { get; set; }
        public string? Color { get; set; } = "";
        public string? Size { get; set; } = "";
        public string? CustomizeInfo { get; set; } = "";
        public string ItemStatus { get; set; }
        public string SellerName { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
       


    }
}
