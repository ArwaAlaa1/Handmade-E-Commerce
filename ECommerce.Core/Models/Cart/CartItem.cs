namespace ECommerce.Core.Models.Cart
{
    public class CartItem
    {
        public string ItemId { get; set; } = Guid.NewGuid().ToString();
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SellerName { get; set; }
        public string SellerId { get; set; }
        public string PhotoUrl { get; set; }
        public string Category { get; set; }
        public string? CustomizeInfo { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal? Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? PriceAfterSale { get; set; } = 0.0m;

        public int ActiveSale { get; set; }
        public int Quantity { get; set; }
    }
}