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
        //OrderItem/ViewOrderItems?orderId=15
        public async Task<IActionResult> ViewOrderItems(int orderId)
        {
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userId = user?.Id;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            _logger.LogInformation($"OrderId: {orderId}, userId: {userId}");
            // Get the order items for the specified orderId and userId
            var orderItems = await _unitOfWork.Repository<OrderItem>()
                                        .GetAllAsync(
                         o => o.OrderId == orderId && o.TraderId == userId,
                         includeProperties: "Product"
                         );

            _logger.LogInformation($"Found {orderItems.Count()} order items for userId: {userId}");

            if (orderItems == null || !orderItems.Any())
                return NotFound();

            return View(orderItems);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrderItem(int orderItemId)
        {
            var orderItem = await _unitOfWork.Repository<OrderItem>()
                .GetFirstOrDefaultAsync(o => o.Id == orderItemId);

            if (orderItem == null)
                return NotFound();


            if (orderItem.OrderItemStatus == ItemStatus.Pending)
            {
                // Change status to Ready
                orderItem.OrderItemStatus = ItemStatus.Ready;
                await _unitOfWork.SaveAsync();
            }

            //Check the status of all OrderItems associated with the order, and if they are all Ready, change the order's status to Ready
            var order = await _unitOfWork.Repository<Order>()
                                         .GetFirstOrDefaultAsync
                                         (
                                             o => o.Id == orderItem.OrderId,
                                             includeProperties: "OrderItems"
                                         );
                                         

            if (order != null && order.OrderItems.All(oi => oi.OrderItemStatus == ItemStatus.Ready))
            {
                // If all OrderItems are in Ready state, change the Order state to Ready
                order.Status = OrderStatus.Ready;
                await _unitOfWork.SaveAsync();
            }
            _logger.LogInformation($"Redirecting to ViewOrderItems with orderId: {orderItem.OrderId}");


            // Redirect to the OrderItems view for the current order
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
                var orderItem = await _unitOfWork.Repository<OrderItem>()
                                                 .GetFirstOrDefaultAsync(o => o.Id == model.OrderItemId,
                                                 includeProperties: "Product");

                if (orderItem == null)
                    return NotFound();
                if (orderItem.Product == null)
                {
                    _logger.LogError("Product is null for OrderItemId: " + model.OrderItemId);
                }
                orderItem.OrderItemStatus = model.OrderItemStatus.Value;
                await _unitOfWork.SaveAsync();

                // Check if all items in the order are Ready, then update the order status
                var order = await _unitOfWork.Repository<Order>()
                    .GetFirstOrDefaultAsync(o => o.Id == orderItem.OrderId);

                if (order != null && order.OrderItems.All(oi => oi.OrderItemStatus == ItemStatus.Ready))
                {
                    order.Status = OrderStatus.Ready;
                    await _unitOfWork.SaveAsync();
                }

                // Redirect to the OrderItems view for the current order
                return RedirectToAction("ViewOrderItems", new { orderId = orderItem.OrderId });
            }
            // Log ModelState errors if invalid
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"ModelState Error: {error.ErrorMessage}");
            }

            return View(model);
        }

    }
}
