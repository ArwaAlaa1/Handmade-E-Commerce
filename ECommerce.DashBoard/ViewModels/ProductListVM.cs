namespace ECommerce.DashBoard.ViewModels
{
    public class ProductListVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal DiscountedPrice { get; set; }

        public string CategoryName { get; set; }

        public bool IsOnSale { get; set; }
        public int? SaleId { get; set; }
    }
}
