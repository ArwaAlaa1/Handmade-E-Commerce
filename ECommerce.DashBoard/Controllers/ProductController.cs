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

            var products = await _unitOfWork.Repository<Product>()
         .GetAllAsync(includeProperties: "Category,Sales");

            var productVMs = products.Select(p =>
            {
                var activeSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                decimal discountedPrice = activeSale != null
                    ? p.Cost * (1 - activeSale.Percent / 100m)
                    : p.Cost;

                return new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Cost = p.Cost,
                    DiscountedPrice = discountedPrice,
                    CategoryName = p.Category?.Name,
                    IsOnSale = activeSale != null
                };
            });

            return View(productVMs);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Repository<Product>()
               .GetByIdWithIncludeAsync(id, "Category,ProductPhotos,Sales");


            if (product == null) return NotFound();
            var currentSale = product.Sales?.FirstOrDefault(s =>
               s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

            var productVM = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Cost = product.Cost,
                CategoryName = product.Category?.Name,
                CategoryId = product.CategoryId,
                ExistingPhotoLinks = product.ProductPhotos?.Select(p => p.PhotoLink).ToList() ?? new List<string>(),
                ExistingPhotoLinksWithIds = product.ProductPhotos?
                    .Select(p => new ProductPhotoVM { Id = p.Id, PhotoLink = p.PhotoLink }).ToList()
                    ?? new List<ProductPhotoVM>(),
                IsOnSale = currentSale != null,
                SaleId = currentSale?.Id,
                SalePercent = currentSale?.Percent,
                SaleStartDate = currentSale?.StartDate,
                SaleEndDate = currentSale?.EndDate,
                DiscountedPrice = currentSale != null
                ? product.Cost - (product.Cost * currentSale.Percent / 100)
                : null
            };

            return View(productVM);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var colors = await _unitOfWork.Repository<Color>().GetAllAsync();
            var sizes = await _unitOfWork.Repository<Size>().GetAllAsync();

            var vm = new ProductVM
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                AvailableColors = colors.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
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
            await _unitOfWork.SaveAsync();

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
                await _unitOfWork.SaveAsync();
            }

            if (vm.SelectedColorIds?.Any() == true)
            {
                foreach (var colorId in vm.SelectedColorIds)
                {
                    await _unitOfWork.Repository<ProductColor>().AddAsync(new ProductColor
                    {
                        ProductId = product.Id,
                        ColorId = colorId
                    });
                }
            }

            if (vm.SelectedSizeIds?.Any() == true)
            {
                foreach (var sizeId in vm.SelectedSizeIds)
                {
                    await _unitOfWork.Repository<ProductSize>().AddAsync(new ProductSize
                    {
                        ProductId = product.Id,
                        SizeId = sizeId
                    });
                }
            }
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            var photos = await _unitOfWork.Repository<ProductPhoto>()
                .GetAllAsync(p => p.ProductId == product.Id);

            var productColorIds = (await _unitOfWork.Repository<ProductColor>()
                .GetAllAsync(pc => pc.ProductId == product.Id)).Select(pc => pc.ColorId).ToList();

            var productSizeIds = (await _unitOfWork.Repository<ProductSize>()
                .GetAllAsync(ps => ps.ProductId == product.Id)).Select(ps => ps.SizeId).ToList();

            var colors = await _unitOfWork.Repository<Color>().GetAllAsync();
            var sizes = await _unitOfWork.Repository<Size>().GetAllAsync();

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
                }).ToList(),
                SelectedColorIds = productColorIds,
                SelectedSizeIds = productSizeIds,
                AvailableColors = colors.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
            };

            return View(vm);
        }


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

                var colors = (await _unitOfWork.Repository<Color>().GetAllAsync()).Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

                var sizes = (await _unitOfWork.Repository<Size>().GetAllAsync()).Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                });

                vm.Categories = categories;
                vm.AvailableColors = colors;
                vm.AvailableSizes = sizes;

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
            }

            var existingProductColors = await _unitOfWork.Repository<ProductColor>()
                .GetAllAsync(pc => pc.ProductId == product.Id);

            foreach (var existing in existingProductColors)
            {
                if (!vm.SelectedColorIds.Contains(existing.ColorId))
                    _unitOfWork.Repository<ProductColor>().Delete(existing);
            }

            foreach (var colorId in vm.SelectedColorIds ?? new List<int>())
            {
                if (!existingProductColors.Any(pc => pc.ColorId == colorId))
                {
                    await _unitOfWork.Repository<ProductColor>().AddAsync(new ProductColor
                    {
                        ProductId = product.Id,
                        ColorId = colorId
                    });
                }
            }

            var existingProductSizes = await _unitOfWork.Repository<ProductSize>()
                .GetAllAsync(ps => ps.ProductId == product.Id);

            foreach (var existing in existingProductSizes)
            {
                if (!vm.SelectedSizeIds.Contains(existing.SizeId))
                    _unitOfWork.Repository<ProductSize>().Delete(existing);
            }

            foreach (var sizeId in vm.SelectedSizeIds ?? new List<int>())
            {
                if (!existingProductSizes.Any(ps => ps.SizeId == sizeId))
                {
                    await _unitOfWork.Repository<ProductSize>().AddAsync(new ProductSize
                    {
                        ProductId = product.Id,
                        SizeId = sizeId
                    });
                }
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int photoId, int productId)
        {
            var photo = await _unitOfWork.Repository<ProductPhoto>().GetByIdAsync(photoId);
            if (photo == null)
                return NotFound();

            // Remove from file system
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, photo.PhotoLink.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            // Remove from database
            _unitOfWork.Repository<ProductPhoto>().Delete(photo);
            _unitOfWork.SaveAsync();

            return RedirectToAction("Edit", new { id = productId });
        }



        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Load product with related Sales
            var product = await _unitOfWork.Repository<Product>()
                .GetByIdWithIncludeAsync(id, "Sales");

            if (product == null) return NotFound();

            // delete related sales
            if (product.Sales != null && product.Sales.Any())
            {
                foreach (var sale in product.Sales.ToList())
                {
                    _unitOfWork.Repository<Sale>().Delete(sale);
                }
            }

            // Delete product
            _unitOfWork.Repository<Product>().Delete(product);

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
