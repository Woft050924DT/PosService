using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BLL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly bll_Category _categoryBll;

        public CategoryController(bll_Category categoryBll)
        {
            _categoryBll = categoryBll;
        }

        // GET: api/category?isActive=true
        [HttpGet]
        public ActionResult<List<CategoryDTO>> GetAll([FromQuery] bool? isActive = null)
        {
            var categories = _categoryBll.GetAllCategory(isActive);
            return Ok(categories);
        }

        // GET: api/category/5
        [HttpGet("{id:int}")]
        public ActionResult<CategoryDTO> GetById(int id)
        {
            var category = _categoryBll.GetCategoryById(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        public ActionResult<bool> Create([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = _categoryBll.CreateCategory(dto);
            if (!ok) return BadRequest();
            return Ok(true);
        }

        // PUT: api/category/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = _categoryBll.UpdateCategory(id, dto);
            if (!ok) return NotFound();
            return Ok(true);
        }

        // DELETE: api/category/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _categoryBll.DeleteCategory(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}           
