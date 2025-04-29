//using ECommerce.Core;
//using ECommerce.Core.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace ECommerce.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BasketsController : ControllerBase
//    {
//        private readonly IUnitOfWork unitOfWork;

//        public BasketsController(IUnitOfWork unitOfWork)
//        {
//            this.unitOfWork = unitOfWork;
//        }

//        [HttpGet("get-basket-item/{id}")]
//        public async Task<IActionResult> GetBasketItem(string id)
//        {
            
//            var result = await unitOfWork.CustomerBaskets.GetBasketAsync(id);

//           if(result is null)
//           {
//                return Ok(new CustomerBasket());
//           }

//            return Ok(result);    
//        }
//        [HttpPost("update-basket")]
//        public async Task<IActionResult> UpdateBasket([FromBody] CustomerBasket basket)
//        {
//            var result = await unitOfWork.CustomerBaskets.UpdateBasketAsync(basket);
//            return Ok(result);
//        }
//        [HttpDelete("delete-basket/{id}")]
//        public async Task<IActionResult> DeleteBasket(string id)
//        {
//            var result = await unitOfWork.CustomerBaskets.DeleteBasketAsync(id);
//            if (result)
//            {
//                return Ok(new
//                {
//                    message = "Basket deleted successfully",
//                    status = 200
//                });
//            }
//            return BadRequest
//                (
//                new
//                {
//                    Message = "Basket not deleted",
//                    Status = 400
//                }
//                );

//        }

//    }
//}
