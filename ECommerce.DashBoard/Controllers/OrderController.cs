using ECommerce.Core;
using ECommerce.Core.Models.Order;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.DashBoard.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = SD.AdminRole)]
        //Order/Index
        public async Task<IActionResult> Index(string status)
        {
            var ordersQuery = _unitOfWork.Repository<Order>().GetAllAsync(includeProperties: "OrderItems.Product,shippingCost");
            var orders = await ordersQuery;

            // Filter data by status
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                // Convert OrderStatus to string and compare
                orders = orders.Where(o => o.Status.ToString().ToLower() == status.ToLower()).ToList();
            }

            var orderVMs = orders.Select(o =>
            {
                var totalAmount = o.OrderItems
                    .Where(oi => oi.Product != null)
                    .Sum(oi => oi.Product.Cost * oi.Quantity);

                var shippingCost = o.shippingCost?.Cost ?? 0;

                return new OrderListVM
                {
                    OrderId = o.Id,
                    CustomerEmail = o.CustomerEmail,
                    TotalAmount = totalAmount + shippingCost,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                };
            }).ToList();

            return View(orderVMs);
        }

        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch request from database
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "OrderItems.Product,shippingCost");

            if (order == null)
            {
                return NotFound();
            }

            var orderVM = new OrderDetailsVM
            {
                OrderId = order.Id,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.OrderItems
                    .Where(oi => oi.Product != null)
                    .Sum(oi => oi.Product.Cost * oi.Quantity) + (order.shippingCost?.Cost ?? 0),
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingCost = order.shippingCost?.Cost ?? 0,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    ProductCost = oi.Product?.Cost ?? 0,
                    TotalCost = (oi.Product?.Cost ?? 0) * oi.Quantity
                }).ToList()
            };

            return View(orderVM);
        }


        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "shippingCost");

            if (order == null)
                return NotFound();

           
            if (order.Status.ToString() != "Pending" && order.Status.ToString() != "InProgress")
                return Forbid();

            var viewModel = new OrderEditVM
            {
                OrderId = order.Id,
                Status = order.Status,
                ShippingCost = order.shippingCost?.Cost ?? 0
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Edit(OrderEditVM model)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == model.OrderId, includeProperties: "shippingCost");

            if (order == null)
                return NotFound();

         
            if (order.Status.ToString() != "Pending" && order.Status.ToString() != "InProgress")
                return Forbid();

            // Modify status
            order.Status = model.Status;

            // Edit shipping cost
            if (order.shippingCost != null)
            {
                order.shippingCost.Cost = model.ShippingCost ?? 0;
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
