using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DashBoard.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var vm = new ProductVM
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                vm.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(vm);
            }

            var product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Cost = vm.Cost,
                CategoryId = vm.CategoryId
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var vm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Cost = product.Cost,
                CategoryId = product.CategoryId,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(vm);
        }

        // ... other code ...

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                var categories = (await _unitOfWork.Repository<Category>().GetAllAsync()).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                vm.Categories = categories;
                return View(vm);
            }

            var product = new Product
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                Cost = vm.Cost,
                CategoryId = vm.CategoryId
            };

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =await  _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();
          
            _unitOfWork.Repository<Product>().Delete(product);
            
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
