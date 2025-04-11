using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.Repository<Product>()
                                            .GetAllAsync();

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Cost,
                CategoryId = p.CategoryId
            });

            return Ok(productDtos);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _unitOfWork.Repository<Product>()
                                           .GetByIdAsync(id);
            if (product == null) return NotFound();

            var dto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Cost,
                CategoryId = product.CategoryId
            };

            return Ok(dto);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product
            {
                Name = dto.Name,
                Cost = dto.Price,
                CategoryId = dto.CategoryId
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await  _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(Get), new { id = product.Id }, dto);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;
            existing.Cost = dto.Price;
            existing.CategoryId = dto.CategoryId;

            _unitOfWork.Repository<Product>().Update(existing);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null) return NotFound();

            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
