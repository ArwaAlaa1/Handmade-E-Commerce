using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs;
using ECommerce.Core.Repository.Contract;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository productRepository;

        public ProductsController(IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            _unitOfWork = unitOfWork;
            this.productRepository = productRepository;
        }

        [HttpGet("GetAllWithFilter")]
        public async Task<IActionResult> GetAllWithFilter(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice)
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

            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, categoryId, maxPrice, minPrice);
            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });

            var allProducts = products.Select(p =>
            {
                var currentSale = p.Sales?.FirstOrDefault(s =>
                    s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today);

                return new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Cost,
                    DiscountedPrice = currentSale != null
                        ? p.Cost - (p.Cost * currentSale.Percent / 100)
                        : (decimal?)null,
                    IsOnSale = currentSale != null,
                    SalePercent = currentSale?.Percent,

                    Category = new
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },

                    Photos = p.ProductPhotos
                        .Where(photo => !photo.IsDeleted)
                        .Select(photo => new
                        {
                            Id = photo.Id,
                            Url = photo.PhotoLink
                        }).ToList(),

                    Colors = p.ProductColors?.Select(c => c.Color).ToList() ?? new List<string>(),

                    Sizes = p.ProductSizes?.Select(s => new SizeDTO
                    {
                        Name = s.Size,
                        ExtraCost = s.ExtraCost
                    }).ToList() ?? new List<SizeDTO>()
                };
            });

            return Ok(allProducts);
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

            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, categoryId, null, null);
            if (products == null || !products.Any())
                return NotFound(new { message = $"No products found for category ID {categoryId}." });

            var productList = products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Cost,
                Category = p.Category.Name,
                Photos = p.ProductPhotos
                          .Where(photo => !photo.IsDeleted)
                          .Select(photo => new { Id = photo.Id, Url = photo.PhotoLink })
                          .ToList()
            });

            return Ok(productList);
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
            if (categoryId <= 0)
                return BadRequest("Invalid category ID. The category ID must be greater than 0.");
            if (pageSize <= 0 || pageIndex <= 0)
                return BadRequest("Invalid pagination parameters. Both pageSize and pageIndex must be greater than 0.");
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId.Value);
            if (category == null)
                return NotFound(new { message = $"Category with ID {categoryId} not found." });

            var products = await productRepository.GetProductsWithOffer(pageSize, pageIndex, categoryId, maxPrice, minPrice);
            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });


            var allProducts = products.Select(p => new
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Cost,
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

            return Ok(allProducts);
        }
        [HttpGet("GetProductsWithActiveOffers")]
        public async Task<IActionResult> GetProductsWithActiveOffers(int pageSize, int pageIndex)
        {
            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, null, null, null);

            var activeOfferProducts = products.Where(p => p.Sales.Any(s =>
                s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today)).Select(p => new
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Cost,
                    Category = new
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name
                    },
                    Offer = p.Sales.Where(s => s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today).Select(s => new
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

            return Ok(activeOfferProducts);
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

            var productDetails = new
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Cost,
                Category = new
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                },
                Offer = product.Sales.Where(s => s.StartDate < DateTime.Now && s.EndDate > DateTime.Now).Select(s => new
                {
                    Id = s.Id,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Discount = s.Percent
                }).ToList(),
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
                }).ToList() ?? new List<SizeDTO>()
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


            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, null, null, null);

            if (products == null || !products.Any())
                return NotFound(new { message = "No products found." });


            var discountedProducts = products
                .Where(p => p.Sales
                    .Any(s => s.Percent >= discountPercentage && !p.IsDeleted && s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today))
                .Select(p =>
                {
                    var currentSale = p.Sales.FirstOrDefault(s => s.StartDate <= DateTime.Today && s.EndDate >= DateTime.Today && s.Percent >= discountPercentage);

                    return new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Cost,
                        DiscountedPrice = currentSale != null
                            ? p.Cost - (p.Cost * currentSale.Percent / 100)
                            : (decimal?)null
                    };
                });


            if (!discountedProducts.Any())
                return NotFound(new { message = $"No products found with a discount of {discountPercentage}% or more." });

            return Ok(discountedProducts);
        }


        /******/
        //// POST: api/products
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] ProductDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var product = new Product
        //    {
        //        Name = dto.Name,
        //        Cost = dto.Price,
        //        CategoryId = dto.CategoryId
        //    };

        //    await _unitOfWork.Repository<Product>().AddAsync(product);
        //    await  _unitOfWork.SaveAsync();

        //    return CreatedAtAction(nameof(Get), new { id = product.Id }, dto);
        //}

        //// PUT: api/products/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        //{
        //    if (id != dto.Id) return BadRequest();
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var existing = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        //    if (existing == null) return NotFound();

        //    existing.Name = dto.Name;
        //    existing.Cost = dto.Price;
        //    existing.CategoryId = dto.CategoryId;

        //    _unitOfWork.Repository<Product>().Update(existing);
        //    await _unitOfWork.SaveAsync();

        //    return NoContent();
        //}

        //// DELETE: api/products/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
        //    if (product == null) return NotFound();

        //    _unitOfWork.Repository<Product>().Delete(product);
        //    await _unitOfWork.SaveAsync();

        //    return NoContent();
        //}

    }
}
