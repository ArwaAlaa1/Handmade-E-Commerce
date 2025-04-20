using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DashBoard.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace ECommerce.DashBoard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ProductController> _logger;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, UserManager<AppUser> userManager, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _unitOfWork.Repository<Product>()
                .GetAllAsync(p => !p.Category.IsDeleted, includeProperties: "Category,Sales");

            var productVMs = products.Select(p =>
            {
                var activeSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now);

                return new ProductListVM
                {
                    Id = p.Id,
                    Name = p.Name,
                    Cost = p.Cost,
                    DiscountedPrice = activeSale != null
                        ? p.Cost * (1 - activeSale.Percent / 100m)
                        : p.Cost,
                    CategoryName = p.Category?.Name,
                    IsOnSale = activeSale != null
                };
            }).ToList();

            return View(productVMs);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _unitOfWork.Repository<Product>()
               .GetByIdWithIncludeAsync(id, "Category,ProductPhotos,Sales,ProductSizes,ProductColors");


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
                //handle colors and sizes
                Colors = product.ProductColors?.Select(pc => new ColorVM
                {
                    Name = pc.Color
                }).ToList() ?? new List<ColorVM>(),

                Sizes = product.ProductSizes?.Select(ps => new SizeVM
                {
                    Name = ps.Size,
                    ExtraCost = ps.ExtraCost
                }).ToList() ?? new List<SizeVM>(),
                //ExistingPhotoLinks = product.ProductPhotos?.Select(p => p.PhotoLink).ToList() ?? new List<string>(),
                //ExistingPhotoLinksWithIds = product.ProductPhotos?
                //    .Select(p => new ProductPhotoVM { Id = p.Id, PhotoLink = p.PhotoLink }).ToList()
                //    ?? new List<ProductPhotoVM>(),
                ExistingPhotoLinks = product.ProductPhotos?
                             .Where(p => !p.IsDeleted)
                             .Select(p => p.PhotoLink)
                             .ToList() ?? new List<string>(),

                                         ExistingPhotoLinksWithIds = product.ProductPhotos?
                             .Where(p => !p.IsDeleted)
                             .Select(p => new ProductPhotoVM { Id = p.Id, PhotoLink = p.PhotoLink })
                             .ToList() ?? new List<ProductPhotoVM>(),
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

            var vm = new ProductVM
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
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

            var user = await _userManager.GetUserAsync(User);

            var product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Cost = vm.Cost,
                CategoryId = vm.CategoryId,
                SellerId = user.Id,
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            };


            // Handle colors
            foreach (var colorVM in vm.Colors)
            {
                product.ProductColors.Add(new ProductColor { Color = colorVM.Name });
            }

            // Handle sizes
            foreach (var sizeVM in vm.Sizes)
            {
                product.ProductSizes.Add(new ProductSize { Size = sizeVM.Name, ExtraCost = sizeVM.ExtraCost });
            }


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

            //if (vm.SelectedColorIds?.Any() == true)
            //{
            //    foreach (var colorId in vm.SelectedColorIds)
            //    {
            //        await _unitOfWork.Repository<ProductColor>().AddAsync(new ProductColor
            //        {
            //            ProductId = product.Id,
            //            ColorId = colorId
            //        });
            //    }
            //}

            //if (vm.SelectedSizeIds?.Any() == true)
            //{
            //    foreach (var sizeId in vm.SelectedSizeIds)
            //    {
            //        await _unitOfWork.Repository<ProductSize>().AddAsync(new ProductSize
            //        {
            //            ProductId = product.Id,
            //            SizeId = sizeId
            //        });
            //    }
            //}
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Edit(int id)
        {
            var product = await _unitOfWork.Repository<Product>()
   .GetByIdWithIncludeAsync(id, "ProductColors,ProductSizes");

            if (product == null) return NotFound();

            var photos = await _unitOfWork.Repository<ProductPhoto>()
                .GetAllAsync(p => p.ProductId == product.Id && !p.IsDeleted);

            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();

            var vm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Cost = product.Cost,
                CategoryId = product.CategoryId,
                Categories = categories?.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }) ?? new List<SelectListItem>(),
                Colors = product.ProductColors?.Select(pc => new ColorVM { Name = pc.Color }).ToList() ?? new List<ColorVM>(),
                Sizes = product.ProductSizes?.Select(ps => new SizeVM { Name = ps.Size, ExtraCost = ps.ExtraCost }).ToList() ?? new List<SizeVM>(),
                ExistingPhotoLinksWithIds = photos?.Select(p => new ProductPhotoVM
                {
                    Id = p.Id,
                    PhotoLink = p.PhotoLink
                }).ToList() ?? new List<ProductPhotoVM>()
            };

            return View(vm);
        }

        //public async Task<IActionResult> Edit(int id)
        //{
        //    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        //    if (product == null) return NotFound();

        //    var photos = await _unitOfWork.Repository<ProductPhoto>()
        //        .GetAllAsync(p => p.ProductId == product.Id);

        //    var productColorIds = (await _unitOfWork.Repository<ProductColor>()
        //        .GetAllAsync(pc => pc.ProductId == product.Id)).Select(pc => pc.ColorId).ToList();

        //    var productSizeIds = (await _unitOfWork.Repository<ProductSize>()
        //        .GetAllAsync(ps => ps.ProductId == product.Id)).Select(ps => ps.SizeId).ToList();

        //    var colors = await _unitOfWork.Repository<Color>().GetAllAsync();
        //    var sizes = await _unitOfWork.Repository<Size>().GetAllAsync();

        //    var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
        //    var vm = new ProductVM
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        Cost = product.Cost,
        //        CategoryId = product.CategoryId,
        //        Categories = categories.Select(c => new SelectListItem
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.Name
        //        }),
        //        ExistingPhotoLinksWithIds = photos.Select(p => new ProductPhotoVM
        //        {
        //            Id = p.Id,
        //            PhotoLink = p.PhotoLink
        //        }).ToList(),
        //        SelectedColorIds = productColorIds,
        //        SelectedSizeIds = productSizeIds,
        //        AvailableColors = colors.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
        //        AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
        //    };

        //    return View(vm);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVM vm)
        {

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"ModelState Error on '{key}': {error.ErrorMessage}");
                    }
                }
                var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                vm.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View(vm);
            }

            var product = await _unitOfWork.Repository<Product>().GetFirstOrDefaultAsync(
                    filter: p => p.Id == vm.Id,
                    includeProperties: "ProductColors,ProductSizes"
                );
            if (product == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Cost = vm.Cost;
            product.CategoryId = vm.CategoryId;
            product.SellerId = user.Id;


            // Delete old ProductColors
            var oldColors = product.ProductColors.ToList();
            foreach (var color in oldColors)
            {
                _unitOfWork.Repository<ProductColor>().Delete(color);
            }

            product.ProductColors.Clear();
            foreach (var colorVM in vm.Colors)
            {
                product.ProductColors.Add(new ProductColor { Color = colorVM.Name, ProductId = product.Id });
            }

            // Delete old ProductSizes
            var oldSizes = product.ProductSizes.ToList();
            foreach (var size in oldSizes)
            {
                _unitOfWork.Repository<ProductSize>().Delete(size);
            }

            product.ProductSizes.Clear();
            foreach (var sizeVM in vm.Sizes)
            {
                product.ProductSizes.Add(new ProductSize { Size = sizeVM.Name, ExtraCost = sizeVM.ExtraCost, ProductId = product.Id });
            }
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveAsync();


            // Save new uploaded photos
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
            }

            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(ProductVM vm)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var categories = (await _unitOfWork.Repository<Category>().GetAllAsync()).Select(c => new SelectListItem
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.Name
        //        });

        //        var colors = (await _unitOfWork.Repository<Color>().GetAllAsync()).Select(c => new SelectListItem
        //        {
        //            Value = c.Id.ToString(),
        //            Text = c.Name
        //        });

        //        var sizes = (await _unitOfWork.Repository<Size>().GetAllAsync()).Select(s => new SelectListItem
        //        {
        //            Value = s.Id.ToString(),
        //            Text = s.Name
        //        });

        //        vm.Categories = categories;
        //        vm.AvailableColors = colors;
        //        vm.AvailableSizes = sizes;

        //        return View(vm);
        //    }

        //    var product = new Product
        //    {
        //        Id = vm.Id,
        //        Name = vm.Name,
        //        Description = vm.Description,
        //        Cost = vm.Cost,
        //        CategoryId = vm.CategoryId
        //    };

        //    _unitOfWork.Repository<Product>().Update(product);
        //    await _unitOfWork.SaveAsync();

        //    // Save uploaded photos
        //    if (vm.Photos != null && vm.Photos.Any())
        //    {
        //        foreach (var photo in vm.Photos)
        //        {
        //            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
        //            var photoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);

        //            using (var stream = new FileStream(photoPath, FileMode.Create))
        //            {
        //                await photo.CopyToAsync(stream);
        //            }

        //            var photoEntity = new ProductPhoto
        //            {
        //                PhotoLink = "/images/products/" + fileName,
        //                ProductId = product.Id
        //            };

        //            await _unitOfWork.Repository<ProductPhoto>().AddAsync(photoEntity);
        //        }
        //    }

        //    var existingProductColors = await _unitOfWork.Repository<ProductColor>()
        //        .GetAllAsync(pc => pc.ProductId == product.Id);

        //    foreach (var existing in existingProductColors)
        //    {
        //        if (!vm.SelectedColorIds.Contains(existing.ColorId))
        //            _unitOfWork.Repository<ProductColor>().Delete(existing);
        //    }

        //    foreach (var colorId in vm.SelectedColorIds ?? new List<int>())
        //    {
        //        if (!existingProductColors.Any(pc => pc.ColorId == colorId))
        //        {
        //            await _unitOfWork.Repository<ProductColor>().AddAsync(new ProductColor
        //            {
        //                ProductId = product.Id,
        //                ColorId = colorId
        //            });
        //        }
        //    }

        //    var existingProductSizes = await _unitOfWork.Repository<ProductSize>()
        //        .GetAllAsync(ps => ps.ProductId == product.Id);

        //    foreach (var existing in existingProductSizes)
        //    {
        //        if (!vm.SelectedSizeIds.Contains(existing.SizeId))
        //            _unitOfWork.Repository<ProductSize>().Delete(existing);
        //    }

        //    foreach (var sizeId in vm.SelectedSizeIds ?? new List<int>())
        //    {
        //        if (!existingProductSizes.Any(ps => ps.SizeId == sizeId))
        //        {
        //            await _unitOfWork.Repository<ProductSize>().AddAsync(new ProductSize
        //            {
        //                ProductId = product.Id,
        //                SizeId = sizeId
        //            });
        //        }
        //    }

        //    await _unitOfWork.SaveAsync();

        //    return RedirectToAction(nameof(Index));
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
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
            await _unitOfWork.SaveAsync();

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
