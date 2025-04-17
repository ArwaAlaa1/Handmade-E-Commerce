using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.DashBoard.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.Controllers
{
    //[Authorize(SD.SuplierRole)]
    [Authorize(Roles = "Supplier")]
    public class OrderItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderItemController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public OrderItemController(IUnitOfWork unitOfWork, ILogger<OrderItemController> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string status = "all")
        {
            var userName = User.Identity?.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user?.Id;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _logger.LogInformation($"Fetching orders for trader with ID: {userId}");

            var allOrders = await _unitOfWork.Repository<Order>()
                .GetAllAsync(
                    o => o.OrderItems.Any(oi => oi.TraderId == userId),
                    includeProperties: "OrderItems,shippingCost,OrderItems.Product"
                );

            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                allOrders = allOrders
                    .Where(o => o.Status.ToString().ToLower() == status.ToLower())
                    .ToList();
            }

            foreach (var order in allOrders)
            {
                var traderOrderItems = order.OrderItems.Where(oi => oi.TraderId == userId).ToList();
                var allReady = traderOrderItems.All(oi => oi.OrderItemStatus == ItemStatus.Ready);
                var anyPending = traderOrderItems.Any(oi => oi.OrderItemStatus == ItemStatus.Pending);

                if (allReady)
                    order.Status = OrderStatus.Ready;
                else if (anyPending)
                    order.Status = OrderStatus.Pending;
                else
                    order.Status = OrderStatus.InProgress;

                await _unitOfWork.SaveAsync();
            }
                var orderListVM = allOrders.Select(o =>
            {
                var totalAmount = o.OrderItems
                    .Where(oi => oi.TraderId == userId && oi.Product != null)
                    .Sum(oi => oi.Quantity * oi.Product.Cost);

                var shippingCost = o.shippingCost?.Cost ?? 0;

                return new OrderListVM
                {
                    OrderId = o.Id,
                    CustomerEmail = o.CustomerEmail,
                    TotalAmount = totalAmount + shippingCost,
                    OrderDate = o.OrderDate,
                    Status = o.Status
                };
            }).ToList();

            return View(orderListVM);
        }

        //OrderItem/ViewOrderItems?orderId=15
        public async Task<IActionResult> ViewOrderItems(int orderId)
        {
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user?.Id;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _logger.LogInformation($"OrderId: {orderId}, userId: {userId}");

           
            var orderItems = await _unitOfWork.Repository<OrderItem>()
                .GetAllAsync(
                    o => o.OrderId == orderId && o.TraderId == userId,
                    includeProperties: "Product"
                );

            _logger.LogInformation($"Found {orderItems.Count()} order items for userId: {userId}");

           
            if (orderItems == null || !orderItems.Any())
            {
                _logger.LogWarning($"No order items found for orderId: {orderId} and userId: {userId}");
                return NotFound("No items found for this order.");
            }

            return View(orderItems);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrderItem(int orderItemId)
        {
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user?.Id;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User is not authenticated.");
                return Unauthorized();
            }
            var orderItem = await _unitOfWork.Repository<OrderItem>()
                .GetFirstOrDefaultAsync(o => o.Id == orderItemId);

            if (orderItem == null)
                return NotFound();

            if (orderItem.OrderItemStatus == ItemStatus.Pending)
            {
                orderItem.OrderItemStatus = ItemStatus.Ready;
                await _unitOfWork.SaveAsync();
            }

           
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(
                    o => o.Id == orderItem.OrderId,
                    includeProperties: "OrderItems" 
                );

            if (order != null)
            {
                var traderOrderItems = order.OrderItems.Where(oi => oi.TraderId == userId).ToList();
                var allReady = traderOrderItems.All(oi => oi.OrderItemStatus == ItemStatus.Ready);
                var anyPending = traderOrderItems.Any(oi => oi.OrderItemStatus == ItemStatus.Pending);

                if (allReady)
                    order.Status = OrderStatus.Ready;
                else if (anyPending)
                    order.Status = OrderStatus.Pending;
                else
                    order.Status = OrderStatus.InProgress;

                await _unitOfWork.SaveAsync();
            }

            _logger.LogInformation($"Redirecting to ViewOrderItems with orderId: {orderItem.OrderId}");
            return RedirectToAction("ViewOrderItems", new { orderId = orderItem.OrderId });
        }

        public async Task<IActionResult> Details(int orderItemId)
        {
            var orderItem = await _unitOfWork.Repository<OrderItem>()
                .GetFirstOrDefaultAsync(o => o.Id == orderItemId, includeProperties: "Product");

            if (orderItem == null)
                return NotFound();

            return View(orderItem);
        }
        public async Task<IActionResult> Edit(int orderItemId)
        {
            var orderItem = await _unitOfWork.Repository<OrderItem>()
                                             .GetFirstOrDefaultAsync(o => o.Id == orderItemId, 
                                             includeProperties: "Product");

            if (orderItem == null)
                return NotFound();
           

            var viewModel = new OrderItemEditVM
            {
                OrderItemId = orderItem.Id,
                OrderItemStatus = orderItem.OrderItemStatus,
                ProductName = orderItem.Product?.Name,
                StatusOptions = Enum.GetValues(typeof(ItemStatus))
                           .Cast<ItemStatus>()
                           .Select(e => new SelectListItem
                           {
                               Value = e.ToString(),
                               Text = e.ToString(),
                               Selected = e == orderItem.OrderItemStatus 
                           })
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(OrderItemEditVM model)
        {
            if (ModelState.IsValid)
            {
               
                var userName = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);
                var userId = user?.Id;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User is not authenticated.");
                    return Unauthorized();
                }

               
                var orderItem = await _unitOfWork.Repository<OrderItem>()
                    .GetFirstOrDefaultAsync(o => o.Id == model.OrderItemId, includeProperties: "Product");

                if (orderItem == null)
                    return NotFound();

                if (orderItem.Product == null)
                {
                    _logger.LogError("Product is null for OrderItemId: " + model.OrderItemId);
                }

               
                orderItem.OrderItemStatus = model.OrderItemStatus.Value;
                await _unitOfWork.SaveAsync();

               
                var order = await _unitOfWork.Repository<Order>()
                    .GetFirstOrDefaultAsync(o => o.Id == orderItem.OrderId, includeProperties: "OrderItems");

                if (order != null)
                {
                   
                    var traderOrderItems = order.OrderItems.Where(oi => oi.TraderId == userId).ToList();

                   
                    var allReady = traderOrderItems.All(oi => oi.OrderItemStatus == ItemStatus.Ready);

                   
                    var anyPending = traderOrderItems.Any(oi => oi.OrderItemStatus == ItemStatus.Pending);

                   
                    if (allReady)
                        order.Status = OrderStatus.Ready;
                    else if (anyPending)
                        order.Status = OrderStatus.Pending;
                    else
                        order.Status = OrderStatus.InProgress;

                    await _unitOfWork.SaveAsync();
                }

                return RedirectToAction("ViewOrderItems", new { orderId = orderItem.OrderId });
            }

           
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"ModelState Error: {error.ErrorMessage}");
            }

            return View(model);
        }


    }
}
