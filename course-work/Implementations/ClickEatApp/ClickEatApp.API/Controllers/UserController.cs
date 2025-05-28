using ClickEatApp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClickEatApp.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ClickEatApp.Data;

namespace ClickEatApp.API.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
    public class UserController : ControllerBase
	{
		private readonly IRepository<User> _userRepository;

		public UserController(IRepository<User> userRepository)
		{
			_userRepository = userRepository;
		}

        [HttpGet("filtered")]
        public async Task<ActionResult<UserListResultDto>> GetAll(
                                        [FromQuery] string? name,
                                        [FromQuery] string? email,
                                        [FromQuery] string? sortBy = "id",
                                        [FromQuery] string? sortDirection = "asc",
                                        [FromQuery] int page = 1,
                                        [FromQuery] int pageSize = 10)
        {
            var users = await _userRepository.GetAllAsync();
            var query = users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(f => f.Name.Trim().ToLowerInvariant().Contains(name.Trim().ToLowerInvariant()));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(f => f.Email.Trim().ToLowerInvariant().Contains(email.Trim().ToLowerInvariant()));
            // Сортиране
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("name", "desc") => query.OrderByDescending(o => o.Name),
                ("name", _) => query.OrderBy(o => o.Name),
                ("email", "desc") => query.OrderByDescending(o => o.Email),
                ("email", _) => query.OrderBy(o => o.Email),
                ("id", "desc") => query.OrderByDescending(o => o.Id),
                ("id", _) => query.OrderBy(o => o.Id),
                _ => query.OrderBy(f => f.Id),
            };

            var total = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(u => new UserViewDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
					DateOfBirth = u.DateOfBirth,
					Gender = u.Gender,
					Password = u.Password,
					Role = u.Role,
                }).ToList();

            return Ok(new UserListResultDto
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
		public async Task<ActionResult<User>> GetUser(int id)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null)
				return NotFound();

			return Ok(user);
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
		{
			var users = await _userRepository.GetAllAsync();
			return Ok(users);
		}

        [HttpPost]
		public async Task<ActionResult> CreateUser([FromBody]RegisterDto dto)
		{
			var user = new User
			{
				Email = dto.Email,
				Name = dto.Name,
				Password = dto.Password,
				DateOfBirth = dto.DateOfBirth,
				Gender = dto.Gender,
			};

			await _userRepository.AddAsync(user);

			return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateUser(int id, [FromBody] EditDto dto)
		{
			var user = await _userRepository.GetByIdAsync(id);
			if (user == null)
				return NotFound();

			user.Name = dto.Name;
			user.Email = dto.Email;
			user.DateOfBirth = dto.DateOfBirth;
			user.Gender = dto.Gender;

			await _userRepository.UpdateAsync(user);

			return NoContent();
		}


		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUser(int id)
		{
			await _userRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
