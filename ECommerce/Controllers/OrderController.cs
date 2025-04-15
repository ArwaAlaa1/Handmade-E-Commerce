using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core.Repository.Contract;
using ECommerce.Core.Services.Contract;
using ECommerce.DTOs.OrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
       
        private readonly ILogger<OrderController> _logger;
       

        public OrderController(IOrderService orderService
            , ICartRepository cartRepository
            , IAddressRepository addressRepository
            , IMapper mapper
            , UserManager<AppUser> userManager
            , IWebHostEnvironment webHostEnvironment,
            ILogger<OrderController> logger,
                        IConfiguration configuration)
        {
            _orderService = orderService;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
            _userManager = userManager;
        
            _logger = logger;
            
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateOrder(OrderDto orderDto)
        {
            try
            {

                var address = await _addressRepository.GetByIdAsync(orderDto.AddressId);

                var user = await _userManager.GetUserAsync(User);
              
                var order = await _orderService.CreateOrderAsync(user.Email, orderDto.CartId, orderDto.ShippingCostId,orderDto.AddressId);
                if (order == null)
                {
                    return BadRequest(new { Message = "Failure in Order Process" });
                }
                var deletedcart = await _cartRepository.DeleteCartAsync(orderDto.CartId);
                return Ok(new { Message= "Order Created Successfully" });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while creating the order.");

                return BadRequest(new { Message = "failed!" });
            }
        }

     
    }
}
