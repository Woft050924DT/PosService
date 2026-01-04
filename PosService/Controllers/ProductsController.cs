
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BLL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly bll_Products _productBll;

        public ProductsController(bll_Products productBll)
        {
            _productBll = productBll;
        }

        // GET: api/products?isActive=true&categoryId=1&supplierId=2&q=search
        [HttpGet]
        public ActionResult<List<ProductDTO>> GetAll([FromQuery] bool? isActive = null,
                                                     [FromQuery] int? categoryId = null,
                                                     [FromQuery] int? supplierId = null,
                                                     [FromQuery] string? q = null)
        {
            var list = _productBll.GetAllProducts(isActive, categoryId, supplierId, q);
            return Ok(list);
        }

        // GET: api/products/5
        [HttpGet("{id:int}")]
        public ActionResult<ProductDTO> GetById(int id)
        {
            var product = _productBll.GetProductById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public ActionResult<bool> Create([FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var ok = _productBll.CreateProduct(dto);
            if (!ok) return BadRequest();
            return Ok(true);
        }

        // PUT: api/products/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProductDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            dto.ProductId = id;
            var ok = _productBll.UpdateProduct(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _productBll.DeleteProduct(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
