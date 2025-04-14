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
            var products = await productRepository.GetProductsWithFilters(pageSize, pageIndex, categoryId, maxPrice, minPrice);

            var allProducts = products.Select(p => new
            {
                Name = p.Name,
                Descreption = p.Description,
                Price = p.Cost,
                Category = new
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name
                },
                Photos = p.ProductPhotos.Select(p => new
                {
                    Id = p.Id,
                    Url = p.PhotoLink
                }).ToList()
            });

            return Ok(allProducts);
        }

        [HttpGet("GetAllWithOffers")]
        public async Task<IActionResult> GetAllWithOffers(int pageSize, int pageIndex, int? categoryId, int? maxPrice, int? minPrice)
        {
            var products = await productRepository.GetProductsWithOffer(pageSize, pageIndex, categoryId, maxPrice, minPrice);

            var allProducts = products.Select(p => new
            {
                Name = p.Name,
                Descreption = p.Description,
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
                }).ToList()
            });

            return Ok(allProducts);
        }

        // GET: api/products/5
        [HttpGet("GetProductByIdWithOffer/{id}")]
        public async Task<IActionResult> GetProductByIdWithOffer(int id)
        {
            var product = await productRepository.GetProductByIDWithOffer(id);

            if (product == null) return NotFound();

            var productDetails = new
            {
                Name = product.Name,
                Descreption = product.Description,
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
                }).ToList()
            };

            return Ok(productDetails);
        }

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
