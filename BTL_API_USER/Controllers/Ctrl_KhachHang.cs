using BLL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace BTL_API_USER.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangsController : ControllerBase
    {
        private BLL_KhachHang BLL_KhachHang;
        public KhachHangsController(BLL_KhachHang _BLL_KhachHang) { 
            BLL_KhachHang=_BLL_KhachHang;
        }
        // POST: api/KhachHangs
        [HttpPost]
        public IActionResult CreateKhachHang([FromBody] Customer model)
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
    }
}
