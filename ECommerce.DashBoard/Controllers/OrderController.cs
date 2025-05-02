using System.Security.Claims;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Repository.Repositories;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.DashBoard.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderController> _logger;
        private readonly UserManager<AppUser> userManager;
        public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            this.userManager = userManager;
        }

        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Index(string status = "all")
        {
            var ordersQuery = _unitOfWork.Repository<Order>().GetAllAsync(includeProperties: "OrderItems,OrderItems.Product,shippingCost");
            var orders = await ordersQuery;

            var orderVMs = orders.Select(o =>
            {
                if (!o.OrderItems.Any())
                {
                    _logger.LogWarning($"No order items found for OrderId: {o.Id}");
                }

                if (o.OrderItems.Any(oi => oi.Product == null))
                {
                    _logger.LogWarning($"Null product found in OrderId: {o.Id}");
                }

               
                //var orderStatus = DetermineOrderStatus(o.OrderItems);


                var totalAmount = o.OrderItems
                    .Where(oi => oi.Product != null)
                    .Sum(oi => oi.Product.SellingPrice * oi.Quantity);

                var shippingCost = o.shippingCost?.Cost ?? 0;

                return new OrderListVM
                {
                    OrderId = o.Id,
                    CustomerEmail = o.CustomerEmail,
                    TotalAmount = totalAmount + shippingCost,
                    OrderDate = o.OrderDate,
                    Status = o.Status 
                    //Status = orderStatus 
                };
            }).ToList();
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                if (Enum.TryParse<OrderStatus>(status, true, out var filterStatus))
                {
                    orderVMs = orderVMs.Where(vm => vm.Status == filterStatus).ToList();
                }
                else
                {
                    _logger.LogWarning($"Invalid status filter: {status}");
                }
            }

            //if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            //{
            //    orderVMs = orderVMs.Where(vm => vm.Status.ToString().ToLower() == status.ToLower()).ToList();
            //}


            return View(orderVMs);
        }

        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Details(int id)
        {
            // Fetch request from database
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "OrderItems,OrderItems.Product,shippingCost");

            if (order == null)
            {
                _logger.LogWarning($"Order not found for OrderId: {id}");
                return NotFound();
            }
            if (order.OrderItems.Any(oi => oi.Product == null))
            {
                _logger.LogWarning($"Null product found in OrderId: {id}");
            }
            var orderaddress= await _unitOfWork.Repository<Address>()
                .GetFirstOrDefaultAsync(o => o.Id == order.ShippingAddressId);
            var traderIds = order.OrderItems.Select(oi => oi.TraderId).Distinct().ToList();
            var traders = await userManager.Users
                .Where(u => traderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName);

            var orderVM = new OrderDetailsVM
            {
                OrderId = order.Id,
                CustomerEmail = order.CustomerEmail,
                Address =  orderaddress.City+ " - "+orderaddress.Region + " - " + orderaddress.AddressDetails,
                TotalAmount = order.OrderItems
            .Where(oi => oi.Product != null)
            .Sum(oi => oi.Product.SellingPrice * oi.Quantity) + (order.shippingCost?.Cost ?? 0),
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingCost = order.shippingCost?.Cost ?? 0,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    ProductCost = oi.Product?.SellingPrice ?? 0,
                    TotalCost = (oi.Product?.SellingPrice ?? 0) * oi.Quantity,
                    TraderName = traders.GetValueOrDefault(oi.TraderId, "Unknown Trader"),
                    OrderItemStatus = oi.OrderItemStatus,
                    CustomizeInfo = oi.CustomizeInfo,
                    Color = oi.Color,
                    Size = oi.Size
                }).ToList()
            };

            return View(orderVM);
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            // Fetch request from database
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "OrderItems,OrderItems.Product,shippingCost");

            if (order == null)
            {
                _logger.LogWarning($"Order not found for OrderId: {id}");
                return NotFound();
            }
            if (order.OrderItems.Any(oi => oi.Product == null))
            {
                _logger.LogWarning($"Null product found in OrderId: {id}");
            }
            var orderaddress = await _unitOfWork.Repository<Address>()
                .GetFirstOrDefaultAsync(o => o.Id == order.ShippingAddressId);
            var traderIds = order.OrderItems.Select(oi => oi.TraderId).Distinct().ToList();
            var traders = await userManager.Users
                .Where(u => traderIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName);

            var orderVM = new OrderDetailsVM
            {
                OrderId = order.Id,
                CustomerEmail = order.CustomerEmail,
                Address = orderaddress.City + " - " + orderaddress.Region + " - " + orderaddress.AddressDetails,
                TotalAmount = order.OrderItems
            .Where(oi => oi.Product != null)
            .Sum(oi => oi.Product.SellingPrice * oi.Quantity) + (order.shippingCost?.Cost ?? 0),
                OrderDate = order.OrderDate,
                Status = order.Status,
                ShippingCost = order.shippingCost?.Cost ?? 0,
                OrderItems = order.OrderItems.Select(oi => new OrderItemVM
                {
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    ProductCost = oi.Product?.SellingPrice ?? 0,
                    TotalCost = (oi.Product?.SellingPrice ?? 0) * oi.Quantity,
                    TraderName = traders.GetValueOrDefault(oi.TraderId, "Unknown Trader"),
                    OrderItemStatus = oi.OrderItemStatus,
                    CustomizeInfo = oi.CustomizeInfo,
                    Color = oi.Color,
                    Size = oi.Size
                }).ToList()
            };

            return View(orderVM);
        }

        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "shippingCost,OrderItems");

            if (order == null)
            {
                _logger.LogWarning($"Order not found for OrderId: {id}");
                return NotFound();
            }


            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress && order.Status != OrderStatus.Ready)
            {
                _logger.LogWarning($"Order {id} cannot be edited. Current status: {order.Status}");
                return Forbid();
                //ViewBag.ErrorMessage = $"Order {id} cannot be edited because it is in {order.Status} status.";
                //return RedirectToAction(nameof(Index));
            }

            var viewModel = new OrderEditVM
            {
                OrderId = order.Id,
                Status = order.Status,
                ShippingCost = order.shippingCost?.Cost ?? 0
            };

            return View(viewModel);
        }

        //[HttpPost]
        //[Authorize(Roles = SD.AdminRole)]
        //public async Task<IActionResult> Edit(OrderEditVM model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogError("Invalid model state for OrderEditVM");
        //        return View(model);
        //    }
        //    var order = await _unitOfWork.Repository<Order>()
        //        .GetFirstOrDefaultAsync(o => o.Id == model.OrderId, includeProperties: "shippingCost,OrderItems");/*OrderItems,shippingCost*/

        //    if (order == null)
        //    {
        //        _logger.LogWarning($"Order not found for OrderId: {model.OrderId}");
        //        return NotFound();
        //    }


        //    if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress)
        //    {
        //        _logger.LogWarning($"Order {model.OrderId} cannot be edited. Current status: {order.Status}");
        //        return Forbid();
        //    }
        //    if (order.OrderItems.Any(oi => oi.Product == null))
        //    {
        //        _logger.LogWarning($"Null product found in OrderId: {model.OrderId}");
        //    }

        //    var orderStatus = DetermineOrderStatus(order.OrderItems);
        //    if (order.Status != orderStatus)
        //    {
        //        order.Status = orderStatus;
        //        _logger.LogInformation($"Order {order.Id} status updated to {orderStatus}");
        //    }

        //    if (/*model.ShippingCost.HasValue &&*/ order.shippingCost != null)
        //    {
        //        order.shippingCost.Cost = model.ShippingCost ?? 0;
        //        //order.shippingCost.Cost = model.ShippingCost.Value;
        //        _logger.LogInformation($"Order {order.Id} shipping cost updated to {model.ShippingCost}");
        //    }

        //    await _unitOfWork.SaveAsync();

        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        [Authorize(Roles = SD.AdminRole)]
        public async Task<IActionResult> Edit(OrderEditVM model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for OrderEditVM");
                return View(model);
            }

            var order = await _unitOfWork.Repository<Order>()
                .GetFirstOrDefaultAsync(o => o.Id == model.OrderId, includeProperties: "shippingCost,OrderItems");

            if (order == null)
            {
                _logger.LogWarning($"Order not found for OrderId: {model.OrderId}");
                return NotFound();
            }

            
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.InProgress && order.Status != OrderStatus.Ready)
            {
                _logger.LogWarning($"Order {model.OrderId} cannot be edited. Current status: {order.Status}");
                ModelState.AddModelError("", "This order cannot be edited because it is not in Pending or In Progress or Ready status.");
                return View(model);
            }

           
            if (order.Status != model.Status)
            {
                order.Status = model.Status;
                _logger.LogInformation($"Order {order.Id} status updated to {model.Status}");

               
                foreach (var item in order.OrderItems)
                {
                    item.OrderItemStatus = model.Status switch
                    {
                        OrderStatus.Pending => ItemStatus.Pending,
                        OrderStatus.InProgress => ItemStatus.InProgress,
                        OrderStatus.Ready => ItemStatus.Ready,
                        OrderStatus.Shipping => ItemStatus.Ready, 
                        OrderStatus.Deliverd => ItemStatus.Ready,
                        OrderStatus.Cancelled => ItemStatus.Cancelled,
                        _ => item.OrderItemStatus 
                    };
                }
                _logger.LogInformation($"All OrderItems for Order {order.Id} updated to match Order Status: {model.Status}");
            }

           
            if (order.shippingCost != null)
            {
                order.shippingCost.Cost = model.ShippingCost ?? 0;
                _logger.LogInformation($"Order {order.Id} shipping cost updated to {model.ShippingCost}");
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        private OrderStatus DetermineOrderStatus(ICollection<OrderItem> items)
        {
            var allCancelled = items.All(oi => oi.OrderItemStatus == ItemStatus.Cancelled);
            var allReady = items.All(oi => oi.OrderItemStatus == ItemStatus.Ready);
            var allInProgress = items.All(oi => oi.OrderItemStatus == ItemStatus.InProgress);
            var anyPending = items.Any(oi => oi.OrderItemStatus == ItemStatus.Pending);

            if (allCancelled)
                return OrderStatus.Cancelled;
            else if (allReady)
                return OrderStatus.Ready;
            else if (allInProgress)
                return OrderStatus.InProgress;
            else if (items.Any(oi => oi.OrderItemStatus == ItemStatus.InProgress) && items.Any(oi => oi.OrderItemStatus == ItemStatus.Ready))
                return OrderStatus.InProgress;
            else if (anyPending)
                return OrderStatus.Pending;

            return OrderStatus.Pending;
        }
    }
}
