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


        //[HttpPost]
        //public async Task<IActionResult> Create(ProductSize ps)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.ProductId = ps.ProductId;
        //        ViewBag.Sizes = new SelectList(await _unitOfWork.Repository<Size>().GetAllAsync(), "Id", "Name");
        //        return View(ps);
        //    }

        //    await _unitOfWork.Repository<ProductSize>().AddAsync(ps);
        //    await _unitOfWork.SaveAsync();
        //    return RedirectToAction("Index", new { productId = ps.ProductId });
        //}

        //public async Task<IActionResult> Edit(int productId, int sizeId)
        //{
        //    var ps = await _unitOfWork.Repository<ProductSize>()
        //        .GetFirstOrDefaultAsync(p => p.ProductId == productId && p.SizeId == sizeId,
        //                                includeProperties: "Size");

        //    if (ps == null) return NotFound();

        //    return View(ps);
        //}

        [HttpPost]
        public async Task<IActionResult> Edit(ProductSize ps)
        {
            if (!ModelState.IsValid)
                return View(ps);

            _unitOfWork.Repository<ProductSize>().Update(ps);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { productId = ps.ProductId });
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(int productId, int sizeId)
        //{
        //    var ps = await _unitOfWork.Repository<ProductSize>()
        //        .GetFirstOrDefaultAsync(p => p.ProductId == productId && p.SizeId == sizeId);

        //    if (ps != null)
        //    {
        //        _unitOfWork.Repository<ProductSize>().Delete(ps);
        //        await _unitOfWork.SaveAsync();
        //    }

        //    return RedirectToAction("Index", new { productId });
        //}
   

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
