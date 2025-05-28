using ClickEatApp.Core;
using ClickEatApp.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using ClickEatApp.API.ImageProcess;
using ClickEatApp.Data;
using ClickEatApp.Services;

namespace ClickEatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin,Restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRepository<Restaurant> _restaurantRepository;

        public RestaurantController(IRepository<Restaurant> restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<RestaurantListResultDto>> GetAll(
                                        [FromQuery] string? name,
                                        [FromQuery] string? ownerName,
                                        [FromQuery] string? sortBy = "id",
                                        [FromQuery] string? sortDirection = "asc",
                                        [FromQuery] int page = 1,
                                        [FromQuery] int pageSize = 10)
        {
            var restaurants = await _restaurantRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(ownerName))
                restaurants = restaurants.Where(r => r.Owner.Name == ownerName);

            var query = restaurants.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(r => r.Name.Trim().ToLowerInvariant().Contains(name.Trim().ToLowerInvariant()));

            // Сортиране
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("name", "desc") => query.OrderByDescending(r => r.Name),
                ("name", _) => query.OrderBy(r => r.Name),
                ("id", "desc") => query.OrderByDescending(r => r.Id),
                ("id", _) => query.OrderBy(r => r.Id),
                _ => query.OrderBy(r => r.Id),
            };

            var total = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(r => new RestaurantViewDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    ImageUrl = r.ImageUrl,
                    IsOpen = r.IsOpen,
                    Description = r.Description,
                    OwnerName = r.Owner.Name,
                    OwnerId = r.OwnerId,
                }).ToList();

            return Ok(new RestaurantListResultDto
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetById(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);
            if (restaurant == null) return NotFound();

            return Ok(restaurant);
        }

        [HttpGet]
        public async Task<ActionResult<Restaurant>> GetAllRestaurants()
        {
            var restaurant = await _restaurantRepository.GetAllAsync();
            if (restaurant == null) return NotFound();

            return Ok(restaurant);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromForm] RestaurantCreateDto restaurantDto)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int ownerId = 0;

            if (role == "Restaurant")
            {
                ownerId = int.Parse(userId!);
            }
            else if (role == "Admin")
            {
                if (restaurantDto.OwnerId == null)
                    return BadRequest("Трябва да избереш собственик.");

                ownerId = restaurantDto.OwnerId.Value;
            }
            var restaurant = new Restaurant
            {
                Name = restaurantDto.Name,
                Description = restaurantDto.Description,
                ImageUrl = PhotoProcess.CreateImage(restaurantDto.Image, "restaurant"),
                IsOpen = restaurantDto.IsOpen,
                OwnerId = ownerId 
            };

            await _restaurantRepository.AddAsync(restaurant);

            return CreatedAtAction(nameof(GetById), new { id = restaurant.Id }, restaurant);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] RestaurantEditDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantDto.Id) return BadRequest();

            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existing = await _restaurantRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            int ownerId;

            if (role == "Admin")
            {
                if (restaurantDto.OwnerId == null)
                    return BadRequest("Трябва да избереш собственик.");
                ownerId = restaurantDto.OwnerId.Value;
            }
            else if (role == "Restaurant" && existing.OwnerId != userId)
            {
                return Forbid();
            }
            else
            {
                ownerId = userId;
            }

            existing.Name = restaurantDto.Name;
            existing.Description = restaurantDto.Description;
            existing.IsOpen = restaurantDto.IsOpen;
            existing.OwnerId = ownerId;

            if (restaurantDto.Image != null && restaurantDto.Image.Length > 0)
            {
                existing.ImageUrl = PhotoProcess.CreateImage(restaurantDto.Image, "restaurant");
            }

            await _restaurantRepository.UpdateAsync(existing);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existing = await _restaurantRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            if (role == "Restaurant" && existing.OwnerId != userId)
                return Forbid();

            await _restaurantRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
