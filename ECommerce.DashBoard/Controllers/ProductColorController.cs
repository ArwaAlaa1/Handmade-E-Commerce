using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductColorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ProductColorController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var colors = await _unitOfWork.Repository<Color>()
                .GetAllAsync(c => c.AppUserId == user.Id);
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            var user = await _userManager.GetUserAsync(User);
            color.AppUserId = user.Id;

            if (!ModelState.IsValid)
                return View(color);

            await _unitOfWork.Repository<Color>().AddAsync(color);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
