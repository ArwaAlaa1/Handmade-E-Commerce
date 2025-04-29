using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs;
using ECommerce.Core.Repository.Contract;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ECommerce.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Services.Utility;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository productRepository;
        private readonly UserManager<AppUser> userManager;

        public ProductsController(IUnitOfWork unitOfWork, IProductRepository productRepository, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.productRepository = productRepository;
            this.userManager = userManager;
        }

        [HttpGet("GetAllWithFilter")]
        public async Task<IActionResult> GetAllWithFilter(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice/*,string? email*/)
        {
            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");
            if (categoryId.HasValue && categoryId <= 0)
                return BadRequest("Invalid category ID. The category ID must be greater than 0.");
            if (maxPrice.HasValue && maxPrice <= 0)
                return BadRequest("Invalid max price. The max price must be greater than 0.");
            if (minPrice.HasValue && minPrice <= 0)
                return BadRequest("Invalid min price. The min price must be greater than 0.");
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest("Invalid price range. The min price must be less than or equal to the max price.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var totalCount = await productRepository.GetFilteredProductsCount(categoryId, maxPrice, minPrice);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, categoryId, maxPrice, minPrice);
            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });

            var productIds = products.Select(p => p.Id).ToList();
            var favoriteProductIds = new List<int>();
            if (userId != null)
            {
                favoriteProductIds = await _unitOfWork.Favorites.GetFavoriteProductIdsAsync(userId, productIds);
            }

            var reviewsCounts = await _unitOfWork.Reviews.GetReviewsCountForProductsAsync(productIds);


            var allProducts = products.Select(p =>
            {
                var currentSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

                decimal basePrice = p.Cost;
                decimal sellingPrice = basePrice + (basePrice * p.AdminProfitPercentage / 100);
                decimal? discountedPrice = null;

                if (currentSale != null)
                {
                    var discount = sellingPrice * currentSale.Percent / 100;
                    discountedPrice = sellingPrice - discount;
                }

                return new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    //AdditionalDetails = p.AdditionalDetails,
                    //BasePrice = basePrice,
                    SellingPrice = sellingPrice,
                    DiscountedPrice = discountedPrice,
                    IsOnSale = currentSale != null,
                    SalePercent = currentSale?.Percent,
                    //IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                    IsFavorite = favoriteProductIds.Contains(p.Id),
                    Rating = reviewsCounts.ContainsKey(p.Id) ? reviewsCounts[p.Id] : 0,
                    Category = new
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },
                    Seller = new
                    {
                        Name = p.Seller.UserName,
                        Email = p.Seller.Email,
                        Photo = p.Seller.Photo
                    },

                    Photos = p.ProductPhotos
                        .Where(photo => !photo.IsDeleted)
                        .Select(photo => new
                        {
                            Id = photo.Id,
                            Url = photo.PhotoLink
                        }).ToList(),

                    //Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),

                    //Sizes = p.ProductSizes?.Select(s => new SizeDTO
                    //{
                    //    Name = s.Size,
                    //    ExtraCost = s.ExtraCost
                    //}).ToList() ?? new List<SizeDTO>()
                };
            });

            //return Ok(allProducts);
            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Products = allProducts
            });
        }


        [HttpGet("GetProductByID/{id}")]
        public async Task<IActionResult> GetByID(int id)
        {
            var pro = await _unitOfWork.Repository<Product>().GetByIdWithIncludeAsync(id, "Category,Seller,ProductColors,ProductPhotos,ProductSizes");// ().GetByIdWithIncludeAsync(id,"");
            //return Ok(pro);

            return  Ok(new
            {
                Id = pro.Id,
                Name = pro.Name,
                Description = pro.Description,
                AdditionalDetails = pro.AdditionalDetails,
                //BasePrice = basePrice,
                SellingPrice = pro.SellingPrice,
                DiscountedPrice = pro.DiscountedPrice,
                IsOnSale = pro.IsOnSale != null,
                SalePercent = pro.IsOnSale,//.Percent,
               // IsFavorite =userId != null && favoriteProductIds.Contains(p.Id),
                Category = new
                {
                    Id = pro.Category.Id,
                    Name = pro.Category.Name
                },
                Seller = new
                {
                    Name = pro.Seller.UserName,
                    Email = pro.Seller.Email,
                    Photo = pro.Seller.Photo
                },

                Photos = pro.ProductPhotos

        [Authorize(Roles =SD.CustomerRole)]
        [HttpGet("GetFavList")]
        public async Task<IActionResult> GetFavList(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice/*,string? email*/)
        {
            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");
            if (categoryId.HasValue && categoryId <= 0)
                return BadRequest("Invalid category ID. The category ID must be greater than 0.");
            if (maxPrice.HasValue && maxPrice <= 0)
                return BadRequest("Invalid max price. The max price must be greater than 0.");
            if (minPrice.HasValue && minPrice <= 0)
                return BadRequest("Invalid min price. The min price must be greater than 0.");
            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest("Invalid price range. The min price must be less than or equal to the max price.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var totalCount = await productRepository.GetFavProductsCount(categoryId, maxPrice, minPrice, userId);
            var products = await productRepository.GetFavProducts(pageSize, pageIndex, categoryId, maxPrice, minPrice, userId);
            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var productIds = products.Select(p => p.Id).ToList();
            //var favoriteProductIds = new List<int>();
            //if (userId != null)
            //{
            //    favoriteProductIds = await _unitOfWork.Favorites.GetFavoriteProductIdsAsync(userId, productIds);
            //}

            var reviewsCounts = await _unitOfWork.Reviews.GetReviewsCountForProductsAsync(productIds);


            var allProducts = products.Select(p =>
            {
                var currentSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

                decimal basePrice = p.Cost;
                decimal sellingPrice = basePrice + (basePrice * p.AdminProfitPercentage / 100);
                decimal? discountedPrice = null;

                if (currentSale != null)
                {
                    var discount = sellingPrice * currentSale.Percent / 100;
                    discountedPrice = sellingPrice - discount;
                }

                return new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    //AdditionalDetails = p.AdditionalDetails,
                    //BasePrice = basePrice,
                    SellingPrice = sellingPrice,
                    DiscountedPrice = discountedPrice,
                    IsOnSale = currentSale != null,
                    SalePercent = currentSale?.Percent,
                    //IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                    //IsFavorite = favoriteProductIds.Contains(p.Id),
                    IsFavorite = true,
                    Rating = reviewsCounts.ContainsKey(p.Id) ? reviewsCounts[p.Id] : 0,
                    Category = new
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },
                    Seller = new
                    {
                        Name = p.Seller.UserName,
                        Email = p.Seller.Email,
                        Photo = p.Seller.Photo
                    },

                    Photos = p.ProductPhotos
                        .Where(photo => !photo.IsDeleted)
                        .Select(photo => new
                        {
                            Id = photo.Id,
                            Url = photo.PhotoLink
                        }).ToList(),


                Colors = pro.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),

                Sizes = pro.ProductSizes?.Select(s => new SizeDTO
                {
                    Name = s.Size,
                    ExtraCost = s.ExtraCost
                }).ToList() ?? new List<SizeDTO>()

                    //Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),

                    //Sizes = p.ProductSizes?.Select(s => new SizeDTO
                    //{
                    //    Name = s.Size,
                    //    ExtraCost = s.ExtraCost
                    //}).ToList() ?? new List<SizeDTO>()
                };
            });

            //return Ok(allProducts);
            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Products = allProducts

            });
        }

        [HttpGet("GetProductsByCategory")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, int pageSize, int pageIndex)
        {
            if (categoryId <= 0)
                return BadRequest("Invalid category ID. The category ID must be greater than 0.");

            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);
            if (category == null)
                return NotFound(new { message = $"Category with ID {categoryId} not found." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favoriteProductIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteProducts = await _unitOfWork.Favorites.GetAllAsync(f => f.UserId == userId);

                if (favoriteProducts?.Any() == true)
                {
                    favoriteProductIds = favoriteProducts.Select(f => f.ProductId).ToList();
                }
            }
            var totalCount = await productRepository.GetFilteredProductsCount(categoryId, null, null);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, categoryId, null, null);
            if (products == null || !products.Any())
                return NotFound(new { message = $"No products found for category ID {categoryId}." });


            var productList = products.Select(p =>
            {
                var currentSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

                decimal basePrice = p.Cost;
                decimal sellingPrice = basePrice + (basePrice * p.AdminProfitPercentage / 100);
                decimal? discountedPrice = null;

                if (currentSale != null)
                {
                    var discount = sellingPrice * currentSale.Percent / 100;
                    discountedPrice = sellingPrice - discount;
                }

                return new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    AdditionalDetails = p.AdditionalDetails,
                    //BasePrice = basePrice,
                    SellingPrice = sellingPrice,
                    DiscountedPrice = discountedPrice,
                    IsOnSale = currentSale != null,
                    SalePercent = currentSale?.Percent,
                    IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                    Category = p.Category.Name,
                    Photos = p.ProductPhotos
                              .Where(photo => !photo.IsDeleted)
                              .Select(photo => new { Id = photo.Id, Url = photo.PhotoLink })
                              .ToList()
                };
            });

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPage = totalPages,
                Products = productList
            });
        }

        [HttpGet("GetAllWithOffers")]
        public async Task<IActionResult> GetAllWithOffers(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice)
        {
            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");

            if (categoryId.HasValue && categoryId <= 0)
                return BadRequest("Invalid category ID. The category ID must be greater than 0.");

            if (maxPrice.HasValue && maxPrice <= 0)
                return BadRequest("Invalid max price. The max price must be greater than 0.");

            if (minPrice.HasValue && minPrice <= 0)
                return BadRequest("Invalid min price. The min price must be greater than 0.");

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
                return BadRequest("Invalid price range. The min price must be less than or equal to the max price.");

            if (categoryId.HasValue)
            {
                var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId.Value);
                if (category == null)
                    return NotFound(new { message = $"Category with ID {categoryId} not found." });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favoriteProductIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteProducts = await _unitOfWork.Favorites.GetAllAsync(f => f.UserId == userId);

                if (favoriteProducts?.Any() == true)
                {
                    favoriteProductIds = favoriteProducts.Select(f => f.ProductId).ToList();
                }
            }

            var totalCount = await productRepository.GetProductsWithOfferCount(categoryId, maxPrice, minPrice);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var products = await productRepository.GetProductsWithOffer(pageSize, pageIndex, categoryId, maxPrice, minPrice);
            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });


            var allProducts = products.Select(p => new
            {
                Name = p.Name,
                Description = p.Description,
                AdditionalDetails = p.AdditionalDetails,
                sellingPrice = p.SellingPrice,
                IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                Category = new
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                },
                Offer = p.Sales.Where(s => s.StartDate < DateTime.Now && s.EndDate > DateTime.Now).Select(s => new
                {
                    Id = s.Id,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Discount = s.Percent
                }).ToList(),
                Photos = p.ProductPhotos.Select(p => new
                {
                    Id = p.Id,
                    Url = p.PhotoLink
                }).ToList(),
                Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),
                Sizes = p.ProductSizes?.Select(s => new SizeDTO
                {
                    Name = s.Size,
                    ExtraCost = s.ExtraCost
                }).ToList() ?? new List<SizeDTO>()
            });

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPage = totalPages,
                Products = allProducts
            });
        }

        [HttpGet("GetProductsWithActiveOffers")]
        public async Task<IActionResult> GetProductsWithActiveOffers(int pageSize, int pageIndex)
        {

            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, null, null, null);
            var activeOfferQuery = products
                .Where(p => p.Sales.Any(s => s.StartDate.Date <= DateTime.Today && s.EndDate.Date >= DateTime.Today));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favoriteProductIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteProducts = await _unitOfWork.Favorites.GetAllAsync(f => f.UserId == userId);

                if (favoriteProducts?.Any() == true)
                {
                    favoriteProductIds = favoriteProducts.Select(f => f.ProductId).ToList();
                }
            }

            int totalCount = activeOfferQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var pagedProducts = activeOfferQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(p =>
                {
                    var currentSale = p.Sales
                        .FirstOrDefault(s => s.StartDate.Date <= DateTime.Today && s.EndDate.Date >= DateTime.Today);

                    decimal basePrice = p.Cost;
                    decimal sellingPrice = basePrice + (basePrice * p.AdminProfitPercentage / 100);
                    decimal discount = currentSale != null ? (sellingPrice * currentSale.Percent / 100) : 0;
                    decimal finalPrice = sellingPrice - discount;

                    return new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        AdditionalDetails = p.AdditionalDetails,
                        SellingPrice = sellingPrice,
                        DiscountedPrice = currentSale != null ? finalPrice : (decimal?)null,
                        SalePercent = currentSale?.Percent,
                        IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                        Category = new
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                        Offer = currentSale == null ? null : new
                        {
                            Id = currentSale.Id,
                            StartDate = currentSale.StartDate,
                            EndDate = currentSale.EndDate,
                            Discount = currentSale.Percent
                        },
                        Photos = p.ProductPhotos
                            .Where(photo => !photo.IsDeleted)
                            .Select(photo => new { photo.Id, Url = photo.PhotoLink }).ToList(),
                        Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),
                        Sizes = p.ProductSizes?.Select(s => new SizeDTO
                        {
                            Name = s.Size,
                            ExtraCost = s.ExtraCost
                        }).ToList() ?? new List<SizeDTO>()
                    };
                }).ToList();


            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Products = pagedProducts
            });
        }

        [HttpGet("GetProductByIdWithOffer/{id}")]
        public async Task<IActionResult> GetProductByIdWithOffer(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID. The product ID must be greater than 0.");


            var product = await productRepository.GetProductByIDWithOffer(id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found." });
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);
            if (category == null)
                return NotFound(new { message = $"Category with ID {product.CategoryId} not found." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favoriteProductIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteProducts = await _unitOfWork.Favorites.GetAllAsync(f => f.UserId == userId);

                if (favoriteProducts?.Any() == true)
                {
                    favoriteProductIds = favoriteProducts.Select(f => f.ProductId).ToList();
                }
            }

            var currentSale = product.Sales.FirstOrDefault(s =>
                              s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

            decimal basePrice = product.Cost;
            decimal sellingPrice = basePrice + (basePrice * product.AdminProfitPercentage / 100);
            decimal discount = currentSale != null ? (sellingPrice * currentSale.Percent / 100) : 0;
            decimal finalPrice = sellingPrice - discount;
            var productDetails = new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                AdditionalDetails = product.AdditionalDetails,
                //BasePrice = basePrice,
                SellingPrice = sellingPrice,
                DiscountedPrice = currentSale != null ? finalPrice : (decimal?)null,
                SalePercent = currentSale?.Percent,
                IsFavorite = userId != null && favoriteProductIds.Contains(product.Id),
                Category = new
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Seller = new
                {
                    Id=product.Seller.Id,
                    Name = product.Seller.UserName,
                    Email = product.Seller.Email,
                    Photo = product.Seller.Photo
                },
                //Offer = product.Sales.Where(s => s.StartDate < DateTime.Now && s.EndDate > DateTime.Now).Select(s => new
                //{
                //    Id = s.Id,
                //    StartDate = s.StartDate,
                //    EndDate = s.EndDate,
                //    Discount = s.Percent
                //}).ToList(),
                Offer = currentSale != null ? new
                {
                    Id = currentSale.Id,
                    StartDate = currentSale.StartDate,
                    EndDate = currentSale.EndDate,
                    Discount = currentSale.Percent
                } : null,
                Photos = product.ProductPhotos.Select(p => new
                {
                    Id = p.Id,
                    Url = p.PhotoLink
                }).ToList(),
                Colors = product.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),
                Sizes = product.ProductSizes?.Select(s => new SizeDTO
                {
                    Name = s.Size,
                    ExtraCost = s.ExtraCost
                }).ToList() ?? new List<SizeDTO>(),
            };

            return Ok(productDetails);
        }
       
        [HttpGet("GetProductsByDiscountPercentage")]
        public async Task<IActionResult> GetProductsByDiscountPercentage(int discountPercentage, int pageSize, int pageIndex)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                return BadRequest("Invalid discount percentage. It must be between 0 and 100.");
            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var favoriteProductIds = new List<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                var favoriteProducts = await _unitOfWork.Favorites.GetAllAsync(f => f.UserId == userId);

                if (favoriteProducts?.Any() == true)
                {
                    favoriteProductIds = favoriteProducts.Select(f => f.ProductId).ToList();
                }
            }
          
            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, null, null, null);


            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });


            var discountedProducts = products
                .Where(p => p.Sales
                    .Any(s => s.Percent == discountPercentage && !p.IsDeleted && s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today))
                .Select(p =>
                {
                    var currentSale = p.Sales
                        .FirstOrDefault(s => s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today && s.Percent == discountPercentage);
                    decimal basePrice = p.Cost;
                    decimal sellingPrice = basePrice + (basePrice * p.AdminProfitPercentage / 100);
                    decimal discount = currentSale != null ? (sellingPrice * currentSale.Percent / 100) : 0;
                    decimal finalPrice = sellingPrice - discount;

                    return new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        AdditionalDetails = p.AdditionalDetails,
                        SellingPrice = sellingPrice,
                        DiscountedPrice = currentSale != null ? finalPrice : (decimal?)null,
                        SalePercent = currentSale?.Percent,
                        IsFavorite = userId != null && favoriteProductIds.Contains(p.Id),
                        Category = new
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        },
                        Offer = new
                        {
                            Id = currentSale?.Id,
                            StartDate = currentSale?.StartDate,
                            EndDate = currentSale?.EndDate,
                            Discount = currentSale?.Percent
                        },
                        Photos = p.ProductPhotos
                            .Where(photo => !photo.IsDeleted)
                            .Select(photo => new { photo.Id, Url = photo.PhotoLink }).ToList(),
                        Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),
                        Sizes = p.ProductSizes?.Select(s => new SizeDTO
                        {
                            Name = s.Size,
                            ExtraCost = s.ExtraCost
                        }).ToList() ?? new List<SizeDTO>()
                    };
                }).ToList();

            
            if (!discountedProducts.Any())
                return NotFound(new { message = $"No products found with a discount of {discountPercentage}%." });

            
            var totalCount = discountedProducts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            
            var pagedProducts = discountedProducts
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            
            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Products = pagedProducts
            });
        }



    }
}
