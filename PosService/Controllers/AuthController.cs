using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PosService.Models;

namespace PosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HDVContext _db;
        private readonly IConfiguration _configuration;

        public AuthController(HDVContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
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
            public string Token { get; set; } = null!;
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

            var token = GenerateToken(user);

            var response = new LoginResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                RoleId = user.RoleId,
                Token = token
            };

            return Ok(response);
        }

        private string GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("uid", user.UserId.ToString())
            };

            if (user.RoleId.HasValue)
                claims.Add(new Claim("roleId", user.RoleId.Value.ToString()));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

