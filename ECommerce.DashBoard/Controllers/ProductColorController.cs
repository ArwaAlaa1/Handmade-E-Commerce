using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductColorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductColorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int productId)
        {
            var productColors = await _unitOfWork.Repository<ProductColor>()
                .GetAllAsync(pc => pc.ProductId == productId, includeProperties: "Color");

            ViewBag.ProductId = productId;
            return View(productColors);
        }

        public async Task<IActionResult> Create(int productId)
        {
            ViewBag.ProductId = productId;
            ViewBag.Colors = new SelectList(await _unitOfWork.Repository<Color>().GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductColor pc)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProductId = pc.ProductId;
                ViewBag.Colors = new SelectList(await _unitOfWork.Repository<Color>().GetAllAsync(), "Id", "Name");
                return View(pc);
            }

            await _unitOfWork.Repository<ProductColor>().AddAsync(pc);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Index", new { productId = pc.ProductId });
        }

        //[HttpPost]
        //public async Task<IActionResult> Delete(int productId, int colorId)
        //{
        //    var color = await _unitOfWork.Repository<ProductColor>()
        //        .GetFirstOrDefaultAsync(pc => pc.ProductId == productId && pc.ColorId == colorId);

        //    if (color != null)
        //    {
        //        _unitOfWork.Repository<ProductColor>().Delete(color);
        //        await _unitOfWork.SaveAsync();
        //    }

        //    return RedirectToAction("Index", new { productId });
        //}
    
    }

}
