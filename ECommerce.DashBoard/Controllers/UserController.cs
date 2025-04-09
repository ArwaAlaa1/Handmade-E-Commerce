using ECommerce.Core.Models;
using ECommerce.DashBoard.Helper;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.DashBoard.Controllers
{
    public class TraderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public TraderController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
               
                return RedirectToAction("Index","Home");
            }
          
            
            return View();
        }
    }
}
