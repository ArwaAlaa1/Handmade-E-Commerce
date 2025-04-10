using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DTOs;

namespace ECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            });

            return Ok(categoryDtos);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null) return NotFound();

            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(dto);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = new Category
            {
                Name = dto.Name
            };

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction(nameof(Get), new { id = category.Id }, dto);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            if (id != dto.Id) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Name = dto.Name;

            _unitOfWork.Repository<Category>().Update(existing);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null) return NotFound();

            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
