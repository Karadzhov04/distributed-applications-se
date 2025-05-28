using ClickEatApp.Core.DTOs;
using ClickEatApp.Web.Services;
using ClickEatApp.Web.ViewModel.Restaurant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClickEatApp.Web.Controllers
{
	public class FoodController : Controller
	{
        private readonly FoodApiService _foodApiService;

        public FoodController(FoodApiService foodApiService)
        {
            _foodApiService = foodApiService;
        }
        public async Task<IActionResult> Index(int restaurantId, string? name, int? minCalories, string? sortBy, string? sortDirection, int page = 1)
        {
            var result = await _foodApiService.GetFilteredFoodsAsync(restaurantId, name, minCalories, sortBy, sortDirection, page, 5);

            result.RestaurantId = restaurantId;

            return View(result);
        }

        [HttpGet]
        public IActionResult Create(int restaurantId)
        {
            var model = new FoodCreateDto
            {
                RestaurantId = restaurantId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(FoodCreateDto foodDto)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Home");

            var result = await _foodApiService.CreateFoodAsync(foodDto, token);

            if (result)
                return RedirectToAction("SeeRestaurant", "Restaurant", new { id = foodDto.RestaurantId });

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var food = await _foodApiService.GetFoodByIdAsync(id, token); 

            return View(food);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FoodEditDto foodDto)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var result = await _foodApiService.EditFoodAsync(foodDto.Id, foodDto, token);
            if (result)
                return RedirectToAction("SeeRestaurant", "Restaurant", new { id = foodDto.RestaurantId });

            return View(foodDto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var result = await _foodApiService.DeleteFoodAsync(id, token);
            return RedirectToAction("Index", "Home");
        }
    }
}
