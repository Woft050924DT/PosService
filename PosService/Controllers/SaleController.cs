
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PosService.DAL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly SalesDAL _salesDal;

        public SalesController(SalesDAL salesDal)
        {
            _salesDal = salesDal;
        }

        // GET: api/sales?customerId=1&from=2025-01-01&to=2025-01-31
        [HttpGet]
        public async Task<ActionResult<List<SalesInvoiceDTO>>> GetAll([FromQuery] int? customerId = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var list = await _salesDal.GetAllAsync(customerId, from, to);
            return Ok(list);
        }

        // GET: api/sales/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SalesInvoiceDTO>> GetById(int id)
        {
            var inv = await _salesDal.GetByIdAsync(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        // POST: api/sales
        // Create a sales invoice (bán hàng)
        [HttpPost]
        public async Task<ActionResult<SalesInvoiceDTO>> Create([FromBody] CreateSalesInvoiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var created = await _salesDal.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.InvoiceId }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // For "xuất bill" you can GET the invoice by id and render it on client or server.
        // GET: api/sales/5/print  -> returns invoice DTO (client generates printable bill)
        [HttpGet("{id:int}/print")]
        public async Task<ActionResult<SalesInvoiceDTO>> Print(int id)
        {
            var inv = await _salesDal.GetByIdAsync(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }
    }
}