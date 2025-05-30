﻿using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs.AddressDtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

using ECommerce.DTOs.IdentityDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ECommerce.Services.Utility;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.CustomerRole)]
    public class UserController : ControllerBase
    {
       
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork ,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.Users
                .Include(u => u.Address)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Photo,
                user.PhoneNumber,
                Address = user.Address ?? new List<Address>()
            });
        }


        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress( [FromBody] AddAddressDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var address = new Address
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Region = dto.Region,
                City = dto.City,
                Country = dto.Country,
                AddressDetails = dto.AddressDetails,
                AppUserId = user.Id
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
        public async Task<IActionResult> GetAllAddress()
        {
            var user = await _userManager.GetUserAsync(User);
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

        [HttpPut("UpdateUserData")]
        public async Task<IActionResult> UpdateUserData( [FromBody] UpdateUserData userdata )
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            user.UserName = userdata.UserName;
            user.PhoneNumber = userdata.Phone;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return Ok();
        }


        [HttpPost("AddUserImage")]
        public async Task<IActionResult> AddUserImage([FromForm]AddPhotoDTO addPhoto)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            if (addPhoto.Photo == null || addPhoto.Photo.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + addPhoto.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await addPhoto.Photo.CopyToAsync(fileStream);
            }
            user.Photo = uniqueFileName;
            await _userManager.UpdateAsync(user);

            return Ok(uniqueFileName);
        }
       

    }
}
