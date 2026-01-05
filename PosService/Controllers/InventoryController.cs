using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BLL;
using PosService.DTO;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly bll_Inventory _inventoryBll;

        public InventoryController(bll_Inventory inventoryBll)
        {
            _inventoryBll = inventoryBll;
        }

        [HttpGet]
        public ActionResult<List<InventoryDTO>> GetAll([FromQuery] bool? isActive = null,
                                                       [FromQuery] int? categoryId = null,
                                                       [FromQuery] int? supplierId = null,
                                                       [FromQuery] string? q = null)
        {
            var list = _inventoryBll.GetAllInventory(isActive, categoryId, supplierId, q);
            return Ok(list);
        }

        [HttpGet("low-stock")]
        public ActionResult<List<InventoryDTO>> GetLowStock([FromQuery] int? categoryId = null,
                                                            [FromQuery] int? supplierId = null,
                                                            [FromQuery] string? q = null)
        {
            var list = _inventoryBll.GetLowStockInventory(categoryId, supplierId, q);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public ActionResult<InventoryDTO> GetById(int id)
        {
            var item = _inventoryBll.GetInventoryById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public ActionResult<bool> Create([FromBody] InventoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            var ok = _inventoryBll.CreateInventory(dto);
            if (!ok) return BadRequest();
            return Ok(true);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] InventoryDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            dto.ProductId = id;
            var ok = _inventoryBll.UpdateInventory(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var ok = _inventoryBll.DeleteInventory(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("stock-out")]
        public ActionResult<List<InventoryStockMovementItemResultDTO>> StockOut([FromBody] InventoryStockMovementDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            try
            {
                var result = _inventoryBll.StockOut(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("stock-in")]
        public ActionResult<InventoryGoodsReceiptResultDTO> StockIn([FromBody] InventoryGoodsReceiptDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto is null) return BadRequest();

            try
            {
                var result = _inventoryBll.ReceiveGoods(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
