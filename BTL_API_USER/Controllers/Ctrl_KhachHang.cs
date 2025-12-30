using System.Data;
using BLL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTL_API_USER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private BLL_KhachHang BLL_KhachHang;
        public KhachHangController(BLL_KhachHang _BLL_KhachHang) { 
            BLL_KhachHang=_BLL_KhachHang;
        }
        // POST: api/KhachHang
        [HttpPost]
        public IActionResult createKhachHang([FromBody] Customer model)
        {
            if (model == null)
            {
                return BadRequest("Model is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                bool result = BLL_KhachHang.createKhachHang(model);

                if (result)
                {
                    return Ok(new { message = "Created successfully", data = model });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to create customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        // Get: api/KhachHang
        [HttpPost("get-by-id")]
        public IActionResult selectKhachHangWithID([FromBody] Customer model)
        {
            if (model == null)
            {
                return BadRequest("Model is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Customer result = BLL_KhachHang.selectKhachHangWithID(model);

                if (result!=null)
                {
                    return Ok(new { message = "Get customer successfully", data = result });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to Get customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        //Patch: api/KhachHang
        [HttpPut]
        public IActionResult updateKhachHangWithID([FromBody] Customer model)
        {
            if (model == null)
            {
                return BadRequest("Model is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                Customer result = BLL_KhachHang.updateKhachHangWithID(model);
                if (result != null)
                {
                    return Ok(new { message = "Update customer successfully", data = result });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to update customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
        //Delete: api/KhachHang
        [HttpDelete]
        public IActionResult deleteKhachHangWithID([FromBody] Customer model)
        {
            if (model == null)
            {
                return BadRequest("Model is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                bool result = BLL_KhachHang.deleteKhachHangWithID(model);
                if (result)
                {
                    return Ok(new { message = "Delete customer successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to delete customer" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error in API: " + ex.Message });
            }
        }
    }
}
