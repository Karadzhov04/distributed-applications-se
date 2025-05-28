using ClickEatApp.Core;
using ClickEatApp.Core.DTOs;
using ClickEatApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClickEatApp.API.ImageProcess;
using System.Globalization;

namespace ClickEatApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FoodController : ControllerBase
	{
		private readonly IFoodRepository _foodRepository;

		public FoodController(IFoodRepository foodRepository)
		{
			_foodRepository = foodRepository;
		}

        // GET: api/food?name=pizza&minCalories=100&sortBy=price&page=1&pageSize=10
        [HttpGet("filtered")]
        public async Task<ActionResult<FoodListResultDto>> GetAll(
                                                                [FromQuery] int restaurantId,
                                                                [FromQuery] string? name,
                                                                [FromQuery] int? minCalories,
                                                                [FromQuery] string? sortBy = "id",
                                                                [FromQuery] string? sortDirection = "asc",
                                                                [FromQuery] int page = 1,
                                                                [FromQuery] int pageSize = 10)
        {
            var allFoods = await _foodRepository.GetAllAsync();

            var query = allFoods.Where(f => f.RestaurantId == restaurantId);
            query = query.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(f => f.Name.Trim().ToLowerInvariant().Contains(name.Trim().ToLowerInvariant()));

            if (minCalories.HasValue)
                query = query.Where(f => f.Calories >= minCalories.Value);

            // Сортиране
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(f => f.Price),
                ("price", _) => query.OrderBy(f => f.Price),
                ("calories", "desc") => query.OrderByDescending(f => f.Calories),
                ("calories", _) => query.OrderBy(f => f.Calories),
                _ => query.OrderBy(f => f.Id),
            };

            var total = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(f => new FoodViewDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    Calories = f.Calories,
                    ImageUrl = f.ImageUrl,
                    Description = f.Description,
                }).ToList();

            return Ok(new FoodListResultDto
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("GetAllByRestaurantId/{restaurantId}")]
        public async Task<ActionResult<List<FoodViewDto>>> GetAllByRestaurantId(int restaurantId)
        {
            var foods = await _foodRepository.GetFoodsByRestaurantIdAsync(restaurantId);

            if (foods == null || !foods.Any())
                return NotFound();

            var result = foods.Select(f => new FoodViewDto
            {
                Id = f.Id,
                Name = f.Name,
                Price = f.Price,
                Description = f.Description,
                Calories = f.Calories,
                ImageUrl = f.ImageUrl,
            }).ToList();

            return Ok(result);
        }


        [HttpGet("{id}")]
		public async Task<ActionResult<Food>> GetById(int id)
		{
			var food = await _foodRepository.GetByIdAsync(id);
			if (food == null) return NotFound();

			return Ok(food);
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromForm] FoodCreateDto foodDto)
		{
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "User")
            {
				return Forbid();
            }

            var food = new Food
            {
                Name = foodDto.Name,
                Description = foodDto.Description,
                ImageUrl = PhotoProcess.CreateImage(foodDto.ImageFile, "food"),
                Calories = foodDto.Calories,
                Price = foodDto.Price,
                CreatedOn = DateTime.UtcNow,
                RestaurantId = foodDto.RestaurantId
            };

            await _foodRepository.AddAsync(food);
			return CreatedAtAction(nameof(GetById), new { id = food.Id }, food);
		}

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] FoodEditDto foodDto)
		{
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "User")
            {
                return Forbid();
            }
            var existing = await _foodRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

			existing.Name = foodDto.Name;
            existing.Description = foodDto.Description;
            existing.Calories = foodDto.Calories;
            existing.Price = foodDto.Price;
            existing.CreatedOn = DateTime.UtcNow;

            if (foodDto.ImageFile != null && foodDto.ImageFile.Length > 0)
            {
                existing.ImageUrl = PhotoProcess.CreateImage(foodDto.ImageFile, "food");
            }

            await _foodRepository.UpdateAsync(existing);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			await _foodRepository.DeleteAsync(id);
			return NoContent();
		}
	}
}
