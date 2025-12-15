using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL;

namespace apiIventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly bll_Products _bll;
        
        public ProductsController(bll_Products bll)
        {
            _bll = bll;
        }
        [HttpGet]
        [Route("get-all")]
        public IActionResult GetProducts() 
        {
            try
            {
                var Products = _bll.GetAllProducts();
                return Ok(Products);
            }
            catch (Exception ex) 
            {
                return StatusCode(500,new { error = ex.Message });
            }
        }

    } 
}
