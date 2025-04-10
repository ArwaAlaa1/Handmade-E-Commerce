using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs.AddressDtos;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetUser/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Photo,
                user.Address,
            });
        }

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress( [FromBody] AddAddressDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var address = new Address
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Region = dto.Region,
                City = dto.City,
                Country = dto.Country,
                AddressDetails = dto.AddressDetails,
                AppUserId = dto.AppUserId
            };

            await _unitOfWork.Repository<Address>().AddAsync(address);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
        }

        [HttpPut("UpdateAddress")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddAddressDto dto)
        {
            if (id == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var address = await _unitOfWork.Repository<Address>().GetByIdAsync(id);
            if (address == null) return NotFound();

            address.FullName = dto.FullName;
            address.PhoneNumber = dto.PhoneNumber;
            address.Region = dto.Region;
            address.City = dto.City;
            address.Country = dto.Country;
            address.AddressDetails = dto.AddressDetails;
            address.AppUserId = address.AppUserId;

            _unitOfWork.Repository<Address>().Update(address);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpGet("GetAllAddress")]
        public async Task<IActionResult> GetAllAddress(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();

            var address = await _unitOfWork.Repository<Address>()
                                    .GetAllAsync(p => p.AppUserId == user.Id);
            if (address == null) return NotFound();

            return Ok(address);
        }

        [HttpGet("GetAddress")]
        public async Task<IActionResult> GetAddress(int id)
        {
            var address = await _unitOfWork.Repository<Address>().GetByIdAsync(id);
            if (address == null)
                return NotFound();

            return Ok(address);
        }

    }
}
