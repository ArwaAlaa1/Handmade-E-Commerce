using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Models.Order;
using ECommerce.DTOs;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingCostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingCostController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //[Authorize(SD.CustomerRole)]
        [HttpGet]
        public async Task<IActionResult> GetAllShippingCosts()
        {
            var costs =await _unitOfWork.Repository<ShippingCost>().GetAllAsync();
            var mappedShipping = _mapper.Map<List<ShippingCostDto>>(costs);
            return Ok(mappedShipping);

        }
      
    }
}
