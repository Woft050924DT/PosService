using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PosService.DAL;
using PosService.DTO;
using PosService.Models;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryDAL _categoryDal;
        private readonly HDVContext _db;

        public CategoryController(CategoryDAL categoryDal, HDVContext db)
        {
            _categoryDal = categoryDal;
            _db = db;
        }

        // GET: api/category?isActive=true
        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetAll([FromQuery] bool? isActive = null)
        {
            var categories = await _categoryDal.GetAllAsync(isActive);
            return Ok(categories);
        }

        // GET: api/category/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await _categoryDal.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> Create([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                IsActive = dto.IsActive ?? true
            };

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync();

            var result = new CategoryDTO
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = entity.CategoryId }, result);
        }

        // PUT: api/category/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (entity == null) return NotFound();

            if (dto.CategoryName != null) entity.CategoryName = dto.CategoryName;
            if (dto.Description != null) entity.Description = dto.Description;
            if (dto.IsActive.HasValue) entity.IsActive = dto.IsActive.Value;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/category/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _db.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (entity == null) return NotFound();

            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}           