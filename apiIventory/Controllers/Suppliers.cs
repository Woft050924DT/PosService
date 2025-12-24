using BLL;

using DTO;
using Microsoft.AspNetCore.Mvc;
namespace BTL_API_ADMIN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly bll_Suppliers _bll;
        public SuppliersController(bll_Suppliers bll)
        {
            _bll = bll;
        }
        [HttpGet]
        [Route("get-all")]
        public IActionResult GetSuppliers()
        {
            try
            {
                var Suppliers = _bll.GetAllSuppliers();
                return Ok(Suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("add")]
        public IActionResult AddSuppliers([FromBody] dto_Suppliers s)
        {
            if (s == null || string.IsNullOrWhiteSpace(s.SupplierName))
                return BadRequest("Dữ liệu không hợp lệ");

            try
            {
                if (_bll.AddSupplier(s))
                    return Ok(new { message = "Thêm thành công" });

                return BadRequest(new { message = "Thêm thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] dto_Suppliers s)
        {
            if (s == null || s.SupplierId <= 0)
                return BadRequest("SupplierId không hợp lệ");

            try
            {
                if (_bll.UpdateSupplier(s))
                    return Ok(new { message = "Cập nhật thành công" });

                return BadRequest(new { message = "Cập nhật thất bại" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{supplierId}")]
        public IActionResult Delete(int supplierId)
        {
            if (supplierId <= 0)
                return BadRequest("SupplierId không hợp lệ");

            try
            {
                if (_bll.DeleteSupplier(supplierId))
                    return Ok(new { message = "Xóa thành công" });

                return NotFound(new { message = "Không tìm thấy Supplier" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
