using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.DashBoard.Data;
using ECommerce.DashBoard.Models;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Repository;
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
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ECommerceDbContext _context;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ECommerceDbContext context)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(includeProperties: "Category");
            var sales = await _unitOfWork.Repository<Sale>().GetAllAsync();
            var orders = await _unitOfWork.Repository<OrderItem>().GetAllAsync(includeProperties: "Product"); // If you have an Order table

            var topProducts = orders
                .GroupBy(o => o.Product.Name)
                .Select(g => new TopProductVM
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToList();

            var categoryStats = products.
                GroupBy(p => p.Category?.Name ?? "Uncategorized")
                .Select(g => new CategoryStatsVM
                {
                    CategoryName = g.Key,
                    ProductCount = g.Count()
                })
                .ToList();

            var activeSalesCount = sales.Count(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

            var model = new DashboardReportVM
            {
                TotalProducts = products.Count(),
                TotalSales = sales.Count(),
                ActiveSales = activeSalesCount,
                TotalRevenue = orders.Sum(o => o.TotalPrice), // assuming this exists
                TopProducts = topProducts,
                PopularCategories = categoryStats
            };

            return View(model);
        }


        //[Authorize]
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
