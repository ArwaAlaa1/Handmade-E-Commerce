using ECommerce.Core.Models;
using ECommerce.DashBoard.Helper;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DashBoard.Controllers
{
    public class TraderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public TraderController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(bool showActiveOnly = false)
        {
            var users = await _userManager.GetUsersInRoleAsync(SD.SuplierRole);
            var traders = showActiveOnly ? users.Where(u => u.IsActive).ToList() : users.ToList();
            return View(traders);
        }

       
        public IActionResult AddTrader()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTrader(TraderVM traderVM)
        {
            if (ModelState.IsValid)
            {
                var imageName=HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                var trader = new AppUser
                {
                    DisplayName = traderVM.DisplayName,
                    UserName = traderVM.UserName,
                    Email = traderVM.Email,
                    PhoneNumber = traderVM.PhoneNumber,
                    Photo = imageName,
                    IsActive = traderVM.IsActive
                };
                var result= await _userManager.CreateAsync(trader, traderVM.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(trader, SD.SuplierRole);

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(traderVM);
                }

                return RedirectToAction("Index");
            }
           

            return View(traderVM);
        }

        public async Task<IActionResult> EditTrader(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            var trader = new TraderVM()
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                PhotoName = user.Photo,
              
            };
            return View(trader);
        }

        [HttpPost]
        public async Task<IActionResult> EditTrader(string id,TraderVM traderVM)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (traderVM.Photo != null)
                {
                    if (user.Photo != null)
                    {
                        HandlerPhotos.DeletePhoto("Users", user.Photo);
                    }
                    var imageName = HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                    user.DisplayName = traderVM.DisplayName;
                    user.DisplayName = traderVM.UserName;
                    user.Email = traderVM.Email;
                    user.PhoneNumber = traderVM.PhoneNumber;
                    user.Photo = imageName;
                    user.IsActive = traderVM.IsActive;
                    
                   
                }
                else
                {
                    user.DisplayName = traderVM.DisplayName;
                    user.DisplayName = traderVM.UserName;
                    user.Email = traderVM.Email;
                    user.PhoneNumber = traderVM.PhoneNumber;
                    user.Photo = traderVM.PhotoName;
                    user.IsActive = traderVM.IsActive;
                }

                 var result = await _userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(traderVM);
        }
    }
}
