namespace ECommerce.DashBoard.ViewModels
{
    public class DashboardReportVM
    {
        public int TotalProducts { get; set; }
        public int TotalSales { get; set; }
        public int ActiveSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TopProductVM> TopProducts { get; set; }
        public List<CategoryStatsVM> PopularCategories { get; set; }
    }

    public class TopProductVM
    {
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
    }

    public class CategoryStatsVM
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
    }

}
