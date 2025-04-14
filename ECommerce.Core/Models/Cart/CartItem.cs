namespace ECommerce.Core.Models.Cart
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public string SellerId { get; set; }
        public string PhotoUrl { get; set; }
        public string Category { get; set; }
        public string CustomizeInfo { get; set; }
        public decimal Price { get; set; }
        public int  ActiveSale { get; set; }
        public int Quantity { get; set; }



    }
}