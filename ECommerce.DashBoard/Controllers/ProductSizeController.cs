using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductSizeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductSizeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productId)
        {
            var productSizes = await _unitOfWork.Repository<ProductSize>()
                .GetAllAsync(ps => ps.ProductId == productId, includeProperties: "Size");

            ViewBag.ProductId = productId;
            return View(productSizes);
        }

        public async Task<IActionResult> Create(int productId)
        {
            ViewBag.ProductId = productId;
            ViewBag.Sizes = new SelectList(await _unitOfWork.Repository<Size>().GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductSize ps)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = ps.ProductId;
                ViewBag.Sizes = new SelectList(await _unitOfWork.Repository<Size>().GetAllAsync(), "Id", "Name");
                return View(ps);
            }

            await _unitOfWork.Repository<ProductSize>().AddAsync(ps);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { productId = ps.ProductId });
        }

        public async Task<IActionResult> Edit(int productId, int sizeId)
        {
            var ps = await _unitOfWork.Repository<ProductSize>()
                .GetFirstOrDefaultAsync(p => p.ProductId == productId && p.SizeId == sizeId,
                                        includeProperties: "Size");

            if (ps == null) return NotFound();

            return View(ps);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductSize ps)
        {
            if (!ModelState.IsValid)
                return View(ps);

            _unitOfWork.Repository<ProductSize>().Update(ps);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { productId = ps.ProductId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int productId, int sizeId)
        {
            var ps = await _unitOfWork.Repository<ProductSize>()
                .GetFirstOrDefaultAsync(p => p.ProductId == productId && p.SizeId == sizeId);

            if (ps != null)
            {
                _unitOfWork.Repository<ProductSize>().Delete(ps);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index", new { productId });
        }
    }

}
