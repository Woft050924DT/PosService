
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosService.DAL;
using PosService.DTO;
using PosService.Models;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductDAL _productDal;
        private readonly HDVContext _db;

        public ProductsController(ProductDAL productDal, HDVContext db)
        {
            _productDal = productDal;
            _db = db;
        }

        // GET: api/products?isActive=true&categoryId=1&supplierId=2&q=search
        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetAll([FromQuery] bool? isActive = null,
                                                                 [FromQuery] int? categoryId = null,
                                                                 [FromQuery] int? supplierId = null,
                                                                 [FromQuery] string? q = null)
        {
            var list = await _productDal.GetAllAsync(isActive, categoryId, supplierId, q);
            return Ok(list);
        }

        // GET: api/products/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productDal.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Create([FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var created = await _productDal.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        // PUT: api/products/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var updated = await _productDal.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _productDal.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}