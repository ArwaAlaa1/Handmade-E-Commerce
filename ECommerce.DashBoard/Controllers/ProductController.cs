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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
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

            // Save uploaded photos
            if (vm.Photos != null && vm.Photos.Any())
            {
                foreach (var photo in vm.Photos)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }

                    var photoEntity = new ProductPhoto
                    {
                        PhotoLink = "/images/products/" + fileName,
                        ProductId = product.Id
                    };

                    await _unitOfWork.Repository<ProductPhoto>().AddAsync(photoEntity);
                }
                _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            var photos = await _unitOfWork.Repository<ProductPhoto>()
        .GetAllAsync(p => p.ProductId == product.Id);

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
                }),
                ExistingPhotoLinksWithIds = photos.Select(p => new ProductPhotoVM
                {
                    Id = p.Id,
                    PhotoLink = p.PhotoLink
                }).ToList()
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

            // Save uploaded photos
            if (vm.Photos != null && vm.Photos.Any())
            {
                foreach (var photo in vm.Photos)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var photoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);

                    using (var stream = new FileStream(photoPath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    var photoEntity = new ProductPhoto
                    {
                        PhotoLink = "/images/products/" + fileName,
                        ProductId = product.Id
                    };

                    await _unitOfWork.Repository<ProductPhoto>().AddAsync(photoEntity);
                }
                _unitOfWork.SaveAsync();
            }

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
