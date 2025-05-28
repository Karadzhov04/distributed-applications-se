using ClickEatApp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClickEatApp.Web.ViewModel.Admin;
using ClickEatApp.Core.Enums;
using ClickEatApp.Web.Services;
using ClickEatApp.Core.DTOs;

namespace ClickEatApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly RestaurantApiService _restaurantApiService;
        private readonly OrderApiService _orderApiService;
        private readonly UserApiService _userApiService;
        public AdminController(IRepository<User> userRepository, RestaurantApiService restaurantApiService, OrderApiService orderApiService, UserApiService userApiService)
        {
            _userRepository = userRepository;
            _restaurantApiService = restaurantApiService;
            _orderApiService = orderApiService;
            _userApiService = userApiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? name, string? email, string? sortBy, string? sortDirection, int page = 1)
        {
            var filteredUsers = await _userApiService.GetFilteredUsersAsync(name, email, sortBy, sortDirection, page, 5);
            
            return View(filteredUsers);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(int userId, RoleEnum newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.Role = newRole;
            await _userRepository.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetAllOrders(string? clientName, string? orderDate, decimal? minPrice, string? sortBy, string? sortDirection, int page = 1)
        {
            DateTime? parsedDate = null;
            if (!string.IsNullOrEmpty(orderDate) && DateTime.TryParse(orderDate, out var date))
            {
                parsedDate = date;
            }


            var result = await _orderApiService.GetFilteredOrdersAsync(clientName, parsedDate, minPrice, sortBy, sortDirection, page, 5);

            return View(result);
        }

        public async Task<IActionResult> GetAllRestaurants(string? name, string? ownerName, string? sortBy, string? sortDirection, int page = 1)
        {
            var result = await _restaurantApiService.GetFilteredRestaurantsAsync(name, ownerName, sortBy, sortDirection, page, 5);

            return View(result);
        }
    }
}

