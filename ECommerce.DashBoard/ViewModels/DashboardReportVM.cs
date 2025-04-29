namespace ECommerce.DashBoard.ViewModels
{
    public class DashboardReportVM
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<BestSellerVM> BestSellers { get; set; }
        public List<RevenueVM> RevenueByProduct { get; set; }
        public List<OrderStatusVM> OrderStatuses { get; set; } // Order status distribution
        public List<TopCustomerVM> TopCustomers { get; set; } // Top customers by orders
        public List<CategoryRevenueVM> CategoryRevenues { get; set; } // Revenue by category
        public string Range { get; set; }
    }

    public class BestSellerVM
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }

    public class RevenueVM
    {
        public string Product { get; set; }
        public decimal Revenue { get; set; }
    }

    public class OrderStatusVM
    {
        public string Status { get; set; }
        public int Count { get; set; }
    }

    public class TopCustomerVM
    {
        public string CustomerEmail { get; set; } // Changed from CustomerName to CustomerEmail
        public int OrderCount { get; set; }
    }

    public class CategoryRevenueVM
    {
        public string Category { get; set; }
        public decimal Revenue { get; set; }
    }
}