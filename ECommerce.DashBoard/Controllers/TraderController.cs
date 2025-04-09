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

        public async Task<IActionResult> Index(string Active ="all")
        {
            var traders = await _userManager.GetUsersInRoleAsync(SD.SuplierRole);
            if (Active !="all")
            {
                traders = Active == "active" ? traders.Where(u => u.IsActive == true).ToList() : traders.Where(u => u.IsActive == false).ToList();
            }
             
            return View(traders);
        }


        public async Task<IActionResult> Details(string id)
        {

            var trader = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            return View(trader);
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
                var imageName = HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                var trader = new AppUser
                {
                    DisplayName = traderVM.DisplayName,
                    UserName = traderVM.UserName,
                    Email = traderVM.Email,
                    PhoneNumber = traderVM.PhoneNumber,
                    Photo = imageName,
                    IsActive = traderVM.IsActive,
                    EmailConfirmed = true,
                };
                var result = await _userManager.CreateAsync(trader, traderVM.Password);
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
        public async Task<IActionResult> EditTrader(string id, TraderVM traderVM)
        {
            if (ModelState.IsValid)
            {
                var traderToUpdate = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (traderVM.Photo != null)
                {
                    if (traderToUpdate.Photo != null)
                    {
                        HandlerPhotos.DeletePhoto("Users", traderToUpdate.Photo);
                    }
                    var imageName = HandlerPhotos.UploadPhoto(traderVM.Photo, "Users");
                    traderToUpdate.DisplayName = traderVM.DisplayName;
                    traderToUpdate.DisplayName = traderVM.UserName;
                    traderToUpdate.Email = traderVM.Email;
                    traderToUpdate.PhoneNumber = traderVM.PhoneNumber;
                    traderToUpdate.Photo = imageName;
                    traderToUpdate.IsActive = traderVM.IsActive;


                }
                else
                {
                    traderToUpdate.DisplayName = traderVM.DisplayName;
                    traderToUpdate.DisplayName = traderVM.UserName;
                    traderToUpdate.Email = traderVM.Email;
                    traderToUpdate.PhoneNumber = traderVM.PhoneNumber;
                    traderToUpdate.Photo = traderVM.PhotoName;
                    traderToUpdate.IsActive = traderVM.IsActive;
                }

                var result = await _userManager.UpdateAsync(traderToUpdate);
                return RedirectToAction("Index");
            }
            return View(traderVM);
        }

        public async Task<IActionResult> DeleteTrader(string id)
        {

            var trader = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            trader.IsActive = false;
            await _userManager.UpdateAsync(trader);
            return RedirectToAction(nameof(Index));
        }
    }
}