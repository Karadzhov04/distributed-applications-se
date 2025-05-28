using ClickEatApp.Core.DTOs;
using ClickEatApp.Web.Services;
using ClickEatApp.Web.ViewModel.Restaurant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClickEatApp.Web.Controllers
{
    //[Authorize(Roles = "Admin,Restaurant")]
    public class RestaurantController : Controller
    {
        private readonly RestaurantApiService _restaurantApiService;
        private readonly UserApiService _userApiService;
        private readonly FoodApiService _foodApiService;

        public RestaurantController(RestaurantApiService restaurantApiService, UserApiService userApiService, FoodApiService foodApiService)
        {
            _restaurantApiService = restaurantApiService;
            _userApiService = userApiService;
            _foodApiService = foodApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new RestaurantCreateVM
            {
                Restaurant = new RestaurantCreateDto(),
                Owners = new List<SelectListItem>()
            };

            var token = HttpContext.Session.GetString("JwtToken");

            if (User.IsInRole("Admin"))
            {
                var users = await _userApiService.GetAllUsersAsync(token);

                viewModel.Owners = users
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = $"{u.Name} ({u.Email})"
                    })
                    .ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RestaurantCreateVM model)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Home");

            var result = await _restaurantApiService.CreateRestaurantAsync(model.Restaurant, token);

            if (result)
                return RedirectToAction("Index", "Home");

            if (User.IsInRole("Admin"))
            {
                var users = await _userApiService.GetAllUsersAsync(token);
                model.Owners = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Email})"
                }).ToList();
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var restaurant = await _restaurantApiService.GetRestaurantById(id, token);
            var users = await _userApiService.GetAllUsersAsync(token);

            var viewModel = new RestaurantCreateVM
            {
                Restaurant = new RestaurantCreateDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    IsOpen = restaurant.IsOpen,
                    OwnerId = restaurant.OwnerId
                },
            };

            viewModel.Owners = users
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = $"{u.Name} ({u.Email})"
                })
                .ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RestaurantEditVM model)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var result = await _restaurantApiService.EditRestaurantAsync(model.Restaurant.Id, model.Restaurant, token);
            if (result)
                return RedirectToAction("Index", "Home");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var result = await _restaurantApiService.DeleteRestaurantAsync(id, token);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SeeRestaurant(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Home"); 

            var restaurant = await _restaurantApiService.GetRestaurantById(id, token);

            var foods = await _foodApiService.GetFoodsByRestaurantIdAsync(id, token);

            restaurant.Foods = foods;

            if (restaurant == null)
                return NotFound();

            return View(restaurant);
        }
    }
}
