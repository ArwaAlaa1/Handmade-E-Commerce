using System.Security.Claims;
using ECommerce.Core.Models.Cart;
using ECommerce.Core.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }
        [HttpPost("CreateOrUpdate")]
        public async Task<ActionResult<Cart>> Create(string cardId, int? shippingCostId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await paymentService.CreateOrUpdatePaymentAsync(cardId, shippingCostId);
            if (result == null)
            {
                return BadRequest(new { message = "Payment creation failed" });
            }
            return Ok(result);
        }
    }
}
