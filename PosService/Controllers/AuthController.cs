using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PosService.Models;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HDVContext _db;

        public AuthController(HDVContext db)
        {
            _db = db;
        }

        public class LoginRequestDto
        {
            public string Username { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public class LoginResponseDto
        {
            public int UserId { get; set; }
            public string Username { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public int? RoleId { get; set; }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid || request == null)
                return BadRequest(ModelState);

            var username = request.Username?.Trim();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username và password không được để trống.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);
            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");

            if (user.PasswordHash != request.Password)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu.");

            var response = new LoginResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleId = user.RoleId
            };

            return Ok(response);
        }
    }
}

