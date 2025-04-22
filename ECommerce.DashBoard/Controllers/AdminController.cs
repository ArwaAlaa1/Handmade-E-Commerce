using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.DashBoard.ViewModels;
using ECommerce.Services.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.DashBoard.Controllers
{
    [Authorize(Roles = SD.AdminRole)]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
      
        public async Task<IActionResult> Index()
        {
            var profitSetting = await _unitOfWork.Repository<ProfitSetting>().GetAllAsync();
            var profitPercentage = profitSetting.FirstOrDefault()?.Percentage ?? 0;

            //ViewBag.AdminProfit = profitPercentage;

            var viewModel = new AdminVM
            {
                ProfitPercentage = profitPercentage
            };

            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfit(ProfitSetting profitSetting)
        {
            if (ModelState.IsValid)
            {
               
                var existingProfitSetting = await _unitOfWork.Repository<ProfitSetting>().GetAllAsync();
                var currentProfitSetting = existingProfitSetting.FirstOrDefault();



                if (currentProfitSetting != null)
                {
                    
                    currentProfitSetting.Percentage = profitSetting.Percentage;
                    _unitOfWork.Repository<ProfitSetting>().Update(currentProfitSetting);
                }
                else
                {

                    await _unitOfWork.Repository<ProfitSetting>().AddAsync(profitSetting);
                }

                await _unitOfWork.SaveAsync();
                //ViewBag.AdminProfit = currentProfitSetting;
                return RedirectToAction("Index");
            }

            return View("Index", new AdminVM { ProfitPercentage = profitSetting.Percentage });
        }
    }
}
