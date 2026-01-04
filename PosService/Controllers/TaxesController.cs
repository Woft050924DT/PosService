using System.Collections.Generic;
using BLL;
using Microsoft.AspNetCore.Mvc;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxesController : ControllerBase
    {
        private readonly bll_Tax _taxBll;

        public TaxesController(bll_Tax taxBll)
        {
            _taxBll = taxBll;
        }

        [HttpGet]
        public ActionResult<List<TaxDTO>> GetAll([FromQuery] bool? isActive = null, [FromQuery] string? q = null)
        {
            var list = _taxBll.GetAllTax(isActive, q);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<TaxDTO> GetById(int id)
        {
            var tax = _taxBll.GetTaxById(id);
            if (tax == null) return NotFound();
            return Ok(tax);
        }

        [HttpPost]
        public ActionResult<bool> Create([FromBody] CreateTaxDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = _taxBll.CreateTax(dto);
            if (!ok) return BadRequest();
            return Ok(true);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UpdateTaxDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = _taxBll.UpdateTax(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _taxBll.DeleteTax(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

