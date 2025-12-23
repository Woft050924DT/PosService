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
    }
}
