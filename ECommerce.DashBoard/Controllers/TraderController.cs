using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.DashBoard.Helper;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Repository;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DashBoard.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class TraderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public TraderController(UserManager<AppUser> userManager , IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string Active ="all")
        {
            var traders = await _userManager.GetUsersInRoleAsync(SD.SuplierRole);
            if (Active !="all")
            {
                traders = Active == "active" ? traders.Where(u => u.IsActive == true).ToList() : traders.Where(u => u.IsActive == false).ToList();
            }
             
            return View(traders);
        }


        public async Task<IActionResult> Details(string id)
        {

            var trader = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return View(trader);
        }

        public IActionResult AddTrader()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTrader(TraderVM traderVM)
        {
            if (ModelState.IsValid)
            {
                var imageName = HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                var trader = new AppUser
                {
                    DisplayName = traderVM.DisplayName,
                    UserName = traderVM.UserName,
                    Email = traderVM.Email,
                    PhoneNumber = traderVM.PhoneNumber,
                    Photo = imageName,
                    IsActive = traderVM.IsActive,
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(trader, traderVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(trader, SD.SuplierRole);

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(traderVM);
                }

                return RedirectToAction("Index");
            }


            return View(traderVM);
        }

        public async Task<IActionResult> EditTrader(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var trader = new TraderVM()
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                PhotoName = user.Photo,

            };
            return View(trader);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrader(string id, TraderVM traderVM)
        {
            if (ModelState.IsValid)
            {
                var traderToUpdate = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (traderVM.Photo != null)
                {
                    if (traderToUpdate.Photo != null)
                    {
                        HandlerPhotos.DeletePhoto("Users", traderToUpdate.Photo);
                    }
                    var imageName = HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                    traderToUpdate.DisplayName = traderVM.DisplayName;
                    traderToUpdate.DisplayName = traderVM.UserName;
                    traderToUpdate.Email = traderVM.Email;
                    traderToUpdate.PhoneNumber = traderVM.PhoneNumber;
                    traderToUpdate.Photo = imageName;
                    traderToUpdate.IsActive = traderVM.IsActive;


                }
                else
                {
                    traderToUpdate.DisplayName = traderVM.DisplayName;
                    traderToUpdate.DisplayName = traderVM.UserName;
                    traderToUpdate.Email = traderVM.Email;
                    traderToUpdate.PhoneNumber = traderVM.PhoneNumber;
                    traderToUpdate.Photo = traderVM.PhotoName;
                    traderToUpdate.IsActive = traderVM.IsActive;
                }

                var result = await _userManager.UpdateAsync(traderToUpdate);
                return RedirectToAction("Index");
            }
            return View(traderVM);
        }

        public async Task<IActionResult> DeleteTrader(string id)
        {

            var trader = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            trader.IsActive = false;
            await _userManager.UpdateAsync(trader);
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Report(string id,string range = "month")
        {

            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(includeProperties: "Category");

            var sales = await _unitOfWork.Repository<Sale>().GetAllAsync();


            // Include OrderItems, Product, and Category
            var ordersQuery = _unitOfWork.Repository<Order>()
                .GetQueryable(include: o => o.Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category));

            // Filter for supplier
             ordersQuery = ordersQuery.Where(o => o.OrderItems.Any(oi => oi.TraderId == id));
            

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

            var dashboardVM = new DashboardReportVM
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                BestSellers = bestSellers,
                RevenueByProduct = revenueByProduct,
                OrderStatuses = orderStatuses,
                TopCustomers = topCustomers,
                CategoryRevenues = categoryRevenues,
                Range = range
            };

            return View(dashboardVM);
        }

    }
}