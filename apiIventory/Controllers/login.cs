using Microsoft.AspNetCore.Mvc;
using DTO;
using BLL;

namespace apiIventory.Controllers
{
    public class LoginController : Controller   // tên class trùng với constructor
    {
        private readonly bll_user bll;

        // Constructor
        public LoginController()
        {
            // tự tạo instance BLL
            this.bll = new bll_user();
        }

        [HttpPost("login")]
        public IActionResult Login(dto_user user)
        {
            var u = bll.Login(user.Username, user.Password);
            if (u == null) return Unauthorized("Username/password incorrect or inactive");

            // return user info + role
            return Ok(new
            {
                u.UserID,
                u.Username,
                u.FullName,
                u.RoleID,
                RoleName = u.Role?.RoleName
            });
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
