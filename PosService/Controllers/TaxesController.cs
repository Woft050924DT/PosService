using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosService.DAL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxesController : ControllerBase
    {
        private readonly TaxDAL _taxDal;

        public TaxesController(TaxDAL taxDal)
        {
            _taxDal = taxDal;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaxDTO>>> GetAll([FromQuery] bool? isActive = null, [FromQuery] string? q = null)
        {
            var list = await _taxDal.GetAllAsync(isActive, q);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaxDTO>> GetById(int id)
        {
            var tax = await _taxDal.GetByIdAsync(id);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpPost]
        public async Task<ActionResult<TaxDTO>> Create([FromBody] CreateTaxDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _taxDal.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.TaxId }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaxDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _taxDal.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _taxDal.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

