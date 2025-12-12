using DTO;
using Microsoft.AspNetCore.Mvc;

namespace apiIventory.Controllers
{
    public class HomeController : Controller
    {
        private readonly dto_helloWorld _helloWorldService;

        public HomeController(dto_helloWorld helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        [HttpGet("send")]
        public IActionResult SendMessage()
        {
            string result = _helloWorldService.sendMess("Hello from DI!");
            return Ok(result);
        }
    }
}
