using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.DashBoard.Data;
using ECommerce.DashBoard.Models;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Repository;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ECommerce.DashBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index(string range = "month")
        {

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(includeProperties: "Category");
            var sales = await _unitOfWork.Repository<Sale>().GetAllAsync(includeProperties: "Product");


            var user = await _userManager.GetUserAsync(User);
            ViewBag.userId = user.Id;
            var isAdmin = User.IsInRole(SD.AdminRole);
            var isSupplier = User.IsInRole(SD.SuplierRole);


            // Include OrderItems, Product, and Category
            var ordersQuery = _unitOfWork.Repository<Order>()
                .GetQueryable(include: o => o.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category));

            // Filter for supplier
            if (isSupplier && user != null)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderItems.Any(oi => oi.TraderId == user.Id));
                products = products.Where(p=> p.SellerId == user.Id);
                sales = sales.Where(s=> s.Product.SellerId == user.Id && s.IsDeleted != true );
            }

            // Filter by range
            var now = DateTimeOffset.UtcNow;
            var filteredOrders = range switch
            {
                "day" => ordersQuery.Where(o => o.OrderDate.Date == now.Date),
                "week" => ordersQuery.Where(o => o.OrderDate >= now.AddDays(-7)),
                "month" => ordersQuery.Where(o => o.OrderDate >= now.AddMonths(-1)),
                "year" => ordersQuery.Where(o => o.OrderDate >= now.AddYears(-1)),
                _ => ordersQuery
            };

            var orders = await filteredOrders.ToListAsync();

            // Existing reports
            var totalOrders = orders.Count();
            var totalRevenue = orders.Sum(o => o.OrderItems.Sum(oi => oi.TotalPrice));

            var bestSellers = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new BestSellerVM
                {
                    Product = g.Key ?? "Unknown",
                    Quantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            var revenueByProduct = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new RevenueVM
                {
                    Product = g.Key ?? "Unknown",
                    Revenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(r => r.Revenue)
                .Take(5)
                .ToList();

            // Order Status Distribution
            var orderStatuses = orders
                .GroupBy(o => o.Status.ToString()) // Use enum's string representation
                .Select(g => new OrderStatusVM
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            // Top Customers by Orders (using CustomerEmail)
            var topCustomers = orders
                .GroupBy(o => o.CustomerEmail ?? "Unknown")
                .Select(g => new TopCustomerVM
                {
                    CustomerEmail = g.Key,
                    OrderCount = g.Count()
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(5)
                .ToList();

            // Revenue by Product Category
            var categoryRevenues = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Product.Category != null ? oi.Product.Category.Name : "Unknown")
                .Select(g => new CategoryRevenueVM
                {
                    Category = g.Key,
                    Revenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(r => r.Revenue)
                .Take(5)
                .ToList();
            var totalproducts = products.Count();
            var totalSales = sales.Count();
            

            var dashboardVM = new DashboardReportVM
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                BestSellers = bestSellers,
                RevenueByProduct = revenueByProduct,
                OrderStatuses = orderStatuses,
                TopCustomers = topCustomers,
                CategoryRevenues = categoryRevenues,
                Range = range,
                TotalProducts = totalproducts,
                TotalSales = totalSales
            };

            return View(dashboardVM);
        }        
        //public async Task<IActionResult> Index()
        //{
        //    //string userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    //var user01 = await _context.Users.FindAsync(userId);

        //    //var role01 = await (from role in _context.Roles
        //    //                  join userRole in _context.UserRoles
        //    //                  on role.Id equals userRole.RoleId
        //    //                  join user in _context.Users
        //    //                  on userRole.UserId equals user.Id
        //    //                  where user.Id == userId
        //    //                  select role)
        //    //      .FirstOrDefaultAsync();


        //    //ViewBag.userData = user01;
        //    //ViewBag.roleData = role01;

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
