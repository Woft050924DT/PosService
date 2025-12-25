using Microsoft.AspNetCore.Mvc;

namespace BTL_API_USER.Controllers
{
    public class Ctrl_HoaDon : Controller
    {
        [Route("api/hoadons")]
        public class ProductController : ControllerBase
        {
            [HttpGet]
            public IActionResult GetAll()
            {
                return Ok("Danh sách sản phẩm");
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                return Ok($"Sản phẩm có ID = {id}");
            }
        }
    }
}
