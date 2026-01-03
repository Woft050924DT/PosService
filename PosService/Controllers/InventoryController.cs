using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosService.DAL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDAL _inventoryDal;

        public InventoryController(InventoryDAL inventoryDal)
        {
            _inventoryDal = inventoryDal;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryDTO>>> GetAll([FromQuery] bool? isActive = null,
                                                                   [FromQuery] int? categoryId = null,
                                                                   [FromQuery] int? supplierId = null,
                                                                   [FromQuery] string? q = null)
        {
            var list = await _inventoryDal.GetAllAsync(isActive, categoryId, supplierId, q);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InventoryDTO>> GetById(int id)
        {
            var item = await _inventoryDal.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryDTO>> Create([FromBody] InventoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var created = await _inventoryDal.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var updated = await _inventoryDal.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _inventoryDal.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

