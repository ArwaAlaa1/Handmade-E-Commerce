namespace ECommerce.DTOs.OrderDtos
{
    public class OneItemInOrderReturnDto
    {

        public int ProductId { get; set; }
        public string Photo { get; set; }
        public string ItemStatus { get; set; }
        public string SellerName { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceAfterSale { get; set; }


    }
}
