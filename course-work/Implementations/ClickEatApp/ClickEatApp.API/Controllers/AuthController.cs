using ClickEatApp.Core;
using ClickEatApp.Core.DTOs;
using ClickEatApp.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ClickEatApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
	{
		private readonly IRepository<User> _userRepository;
		private readonly IConfiguration _configuration;

		public AuthController(IRepository<User> userRepository, IConfiguration configuration)
		{
			_userRepository = userRepository;
			_configuration = configuration;
		}

        [HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			var user = (await _userRepository.GetAllAsync())
				.FirstOrDefault(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

			if (user == null)
				return Unauthorized("Invalid credentials");

			var token = GenerateJwtToken(user);
			return Ok(new { Token = token });
		}

		private string GenerateJwtToken(User user)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings");

			var claims = new[]
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(RoleEnum), user.Role))
            };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}

