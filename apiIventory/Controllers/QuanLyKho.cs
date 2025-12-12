using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL;

namespace apiIventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly bll_Categories _bll;

        // Constructor: nhận BLL qua DI
        public CategoriesController(bll_Categories bll)
        {
            _bll = bll;
        }

        // GET: api/categories
        [Route("get-all")]
        [HttpGet]

        public IActionResult GetCategories()
        {
            try
            {
                var categories = _bll.GetAllCategories();
                return Ok(categories); // trả về JSON với HTTP 200
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: api/categories/add
        [Route("add")]
        [HttpPost]
        public IActionResult AddCategory([FromBody] dto_Categories category)
        {
            try
            {
                bool result = _bll.CreateCategory(category);

                if (result)
                    return Ok(new { message = "Thêm loai hang thành công", data = category });

                return BadRequest(new { message = "Không thể thêm Loai hang" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Route("delete/{categoryId}")]
        [HttpDelete]
        public IActionResult DeleteCategory(int categoryId)
        {
            try
            {
                bool result = _bll.DeleteCategory(categoryId);
                if (result)
                    return Ok(new { message = "Xóa loại hàng thành công", CategoryID = categoryId });
                return BadRequest(new { message = "Không thể xóa loại hàng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
