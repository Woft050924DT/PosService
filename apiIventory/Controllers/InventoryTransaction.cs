using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL;
namespace BTL_API_ADMIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly bll_InventoryTransaction _bll;
        public InventoryController(bll_InventoryTransaction bll)
        {
            _bll = bll;
        }
        [HttpGet]
        [Route("get-all")]
        public IActionResult GetProducts()
        {
            try
            {
                var Inventory = _bll.GetAll();
                return Ok(Inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost]
        [Route("add")]
        public IActionResult AddTransaction([FromBody] dto_InventoryTransaction transaction)
        {
            try
            {
                bool isAdded = _bll.AddTransaction(transaction);
                if (isAdded)
                {
                    return Ok(new { message = "Thêm thành công" });
                }
                else
                {
                    return StatusCode(500, new { error = "Thêm thất bại" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPut]
        [Route("update")]
        public IActionResult UpdateTransaction([FromBody] dto_InventoryTransaction transaction)
        {
            try
            {
                bool isUpdated = _bll.UpdateTransaction(transaction);
                if (isUpdated)
                {
                    return Ok(new { message = "Cập nhật thành công" });
                }
                else
                {
                    return StatusCode(500, new { error = "Cập nhật thất bại" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpDelete]
        [Route("delete/{TransactionId}")]
        public IActionResult DeleteTransaction(int TransactionId) 
        {
            if (TransactionId <= 0)
                return BadRequest(new { error = "Id không hợp lệ" });

            try
            {
                bool isDeleted = _bll.DeleteTransaction(TransactionId);
                if (isDeleted)
                {
                    return Ok(new { message = "Xoá thành công" });
                }
                else
                {
                    return StatusCode(500, new { error = "Xoá thất bại" });
                }
            }
            catch (Exception ex)
            {
                return NotFound(new { error = "Không tìm thấy giao dịch để xóa" });
            }
        }

    }
}
