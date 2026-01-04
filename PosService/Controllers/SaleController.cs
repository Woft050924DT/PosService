using System;
using System.Collections.Generic;
using BLL;
using Microsoft.AspNetCore.Mvc;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly bll_Sales _salesBll;

        public SalesController(bll_Sales salesBll)
        {
            _salesBll = salesBll;
        }

        // GET: api/sales?customerId=1&from=2025-01-01&to=2025-01-31
        [HttpGet]
        public ActionResult<List<SalesInvoiceDTO>> GetAll([FromQuery] int? customerId = null, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var list = _salesBll.GetAllSales(customerId, from, to);
            return Ok(list);
        }

        // GET: api/sales/5
        [HttpGet("{id:int}")]
        public ActionResult<SalesInvoiceDTO> GetById(int id)
        {
            var inv = _salesBll.GetSalesInvoiceById(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }

        // POST: api/sales
        // Create a sales invoice (bán hàng)
        [HttpPost]
        public ActionResult<bool> Create([FromBody] CreateSalesInvoiceDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var ok = _salesBll.CreateSalesInvoice(dto);
                if (!ok) return BadRequest();
                return Ok(true);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // For "xuất bill" you can GET the invoice by id and render it on client or server.
        // GET: api/sales/5/print  -> returns invoice DTO (client generates printable bill)
        [HttpGet("{id:int}/print")]
        public ActionResult<SalesInvoiceDTO> Print(int id)
        {
            var inv = _salesBll.GetSalesInvoiceById(id);
            if (inv == null) return NotFound();
            return Ok(inv);
        }
    }
}
