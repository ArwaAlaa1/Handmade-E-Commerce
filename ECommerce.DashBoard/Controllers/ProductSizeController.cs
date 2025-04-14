using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductSizeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public ProductSizeController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var sizes = await _unitOfWork.Repository<Size>()
                .GetAllAsync(s => s.AppUserId == user.Id);
            return View(sizes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            var user = await _userManager.GetUserAsync(User);
            size.AppUserId = user.Id;

            if (!ModelState.IsValid)
                return View(size);

            await _unitOfWork.Repository<Size>().AddAsync(size);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }

}
