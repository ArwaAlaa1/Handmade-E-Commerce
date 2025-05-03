using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Repository.Contract;
using ECommerce.Core.Services.Contract;
using ECommerce.DTOs;
using ECommerce.DTOs.OrderDtos;
using ECommerce.Hubs;
using ECommerce.Repository.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly ILogger<OrderController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;

        public OrderController(IOrderService orderService
            , ICartRepository cartRepository
            , IAddressRepository addressRepository
            , IMapper mapper
            , UserManager<AppUser> userManager
            , IWebHostEnvironment webHostEnvironment,
            ILogger<OrderController> logger,
                        IConfiguration configuration,
                        IUnitOfWork unitOfWork,

                        IHubContext<NotificationHub> hubContext
            , INotificationRepository notificationRepository)


        {
            _orderService = orderService;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
            _userManager = userManager;
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;


            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDto orderDto)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(orderDto.AddressId);

                if (address == null)
                {
                    return BadRequest(new { Message = "Address Not Found" });
                }
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return BadRequest(new { Message = "User Not Found" });
                }
                var order = await _orderService.CreateOrderAsync(user.Email, orderDto.CartId, orderDto.ShippingCostId,orderDto.AddressId,orderDto.PaymentId);

         
                if (order == null)
                {
                    return BadRequest(new { Message = "Failure in Order Process" });
                }

                // Delete the cart
                var deletedcart = await _cartRepository.DeleteCartAsync(orderDto.CartId);

                if (!deletedcart)
                {
                    return BadRequest(new { Message = "Failure in Cart Deletion" });
                }


                // Send notification to each unique trader in the order
                var traderIds = order.OrderItems
                                     .Select(oi => oi.TraderId)
                                     .Distinct()
                                     .ToList();

                foreach (var traderId in traderIds)
                {
                    await _notificationRepository.AddNotificationAsync(new Notification
                    {
                        AppUserId = traderId,
                        Message = "You have a new order!",
                    });

                    await _hubContext.Clients
                        .Group($"Trader_{traderId}")
                        .SendAsync("ReceiveOrderNotification", new
                        {
                            Message = "🔔 You have a new order!",
                            OrderId = order.Id,
                            TraderId = traderId
                        });
                }

                return Ok(new { Message= "Order Created Successfully",orderId=order.Id });
              

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the order.");


                return BadRequest(new { Message = "Failed to create order.", Error = ex.Message });

             
            }
        }

        [Authorize]
        [HttpGet("UserOrders")]
        public async Task<ActionResult<IEnumerable<OrderReturnDto>>> GetUserOrders()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                
                var orders = await _orderService.GetOrdersForUserAsync(user.Email);
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { Message = "User Not Have Orders !" });
                }


                var mappedOrders = _mapper.Map<List<OrderReturnDto>>(orders);


                return Ok(mappedOrders);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while retrieving orders for user.");


                return BadRequest(new { Message = "Try Another Time !" });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OneOrderReturnDto>> GetOrderForUser(int id)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);


                var order = await _orderService.GetOrderForUserAsync(id);


                if (order == null)
                {
                    _logger.LogWarning($"Order with id {id} not found for user {user?.Email}");
                    return NotFound(new { Message = "الطلب غير موجود" });
                }


                var mappedOrder = _mapper.Map<OneOrderReturnDto>(order);


                return Ok(mappedOrder);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"An error occurred while retrieving order with id {id}");


                return BadRequest(new { Message = "Try Another Time!." });
            }
        }

        [Authorize]
        [HttpPost("CancelOrder")]
        public async Task<ActionResult> CancelOrder(int orderId)
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);
                var order = await _orderService.GetOrderForUserAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { Message = "order not exsist" });
                }
                else if (order.OrderItems.Any(x => x.OrderItemStatus == ItemStatus.InProgress))
                {
                    return BadRequest(new { Message = "You Can't Cancel Order ,Order Already Is InProgress" });
                }else if(order.OrderItems.All(x => x.OrderItemStatus == ItemStatus.Pending))
                {
                      await _orderService.CancelOrder(orderId);
                    return Ok(new { Message = "Order Canceled Successfully" });
                }
                else if (order.OrderItems.Any(x => x.OrderItemStatus == ItemStatus.Ready))
                {
                    return Ok(new { Message = "Order  Can't Canceled , Order Already Is Ready" });
                }
                return Ok(new { Message = "Order  Can't Canceled " });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling the order.");
                return BadRequest(new { Message = "Try Another Time !" });
            }
        }

        [Authorize]
        [HttpPost("CancelItem")]
        public async Task<ActionResult> CancelItemInOrder(int orderItemId)
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);
                var orderitem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId);
                if (orderitem == null)
                {
                    return NotFound(new { Message = "order not exsist" });
                }
                else if (orderitem.OrderItemStatus == ItemStatus.InProgress)
                {
                    return BadRequest(new { Message = "You Can't Cancel Item ,Item Already InProgress" });
                }
                else if (orderitem.OrderItemStatus == ItemStatus.Pending)
                {
                    await _orderService.CancelItemOrder(orderItemId);
                    return Ok(new { Message = "Item Canceled Successfully" });
                }
                else if (orderitem.OrderItemStatus == ItemStatus.Ready)
                {
                    
                    return Ok(new { Message = "Item Already Ready Successfully" });
                }
                return Ok(new { Message = "Failed Cancel Item " });


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling Item.");
                return BadRequest(new { Message = "Try Another Time !" });
            }
        }

    }


}
