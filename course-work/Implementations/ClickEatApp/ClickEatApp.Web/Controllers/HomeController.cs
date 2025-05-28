using ClickEatApp.Core.DTOs;
using ClickEatApp.Web.Services;
using ClickEatApp.Web.ViewModel.Home;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ClickEatApp.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly AuthApiService _authApiService;
		private readonly RestaurantApiService _restaurantApiService;

		public HomeController(AuthApiService authApiService, RestaurantApiService restaurantApiService)
		{
			_authApiService = authApiService;
            _restaurantApiService = restaurantApiService;

        }

        [HttpGet]
        public IActionResult DefaultPage()
        {
            return View();
        }

        [HttpGet]
		public async Task<IActionResult> Index()
		{
            var token = HttpContext.Session.GetString("JwtToken");

            var restaurants = await _restaurantApiService.GetAllRestaurantsAsync(token);
            var listRestaurantsVM = restaurants.Select(r => new RestaurantViewDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsOpen = r.IsOpen,
                ImageUrl = r.ImageUrl,
            }).ToList();


            return View(listRestaurantsVM);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			if (!ModelState.IsValid)
                return View(loginDto);

			var token = await _authApiService.LoginUserAsync(loginDto);

			if (token == null)
			{
				ModelState.AddModelError(string.Empty, "Невалидни данни за вход.");
				return View(loginDto);
			}

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var claims = new List<Claim>();
            foreach (var claim in jwtToken.Claims)
            {
                var claimType = claim.Type == "role" ? ClaimTypes.Role : claim.Type;
                claims.Add(new Claim(claimType, claim.Value));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("JwtToken", token);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterDto user)
		{
			var result = await _authApiService.RegisterUserAsync(user);
			if (result)
				return RedirectToAction("Login");

			return View(user);
		}

        [HttpGet]
        public IActionResult Logout()
        {
            this.HttpContext.Session.Remove("JwtToken");

            return RedirectToAction("DefaultPage", "Home");
        }

    }
}
