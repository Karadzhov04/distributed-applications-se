using ClickEatApp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using ClickEatApp.Web.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClickEatApp.Web.ViewModel.User;

public class UserController : Controller
{
	private readonly UserApiService _userApiService;
	private readonly OrderApiService _orderApiService;

	public UserController(UserApiService userApiService, OrderApiService orderApiService )
	{
		_userApiService = userApiService;
        _orderApiService = orderApiService;
	}

	public async Task<IActionResult> Index(int id)
	{
		var token = HttpContext.Session.GetString("JwtToken");

		return View();
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		var token = HttpContext.Session.GetString("JwtToken");
		var user = await _userApiService.GetUserById(id, token);

		var userDto = new UserEditDto
		{
			Id = user.Id,
			Name = user.Name,
			Email = user.Email,
			Password = user.Password,
			Gender = user.Gender,
			DateOfBirth = user.DateOfBirth,
		};

		return View(userDto);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(UserEditDto user)
	{
		var token = HttpContext.Session.GetString("JwtToken");
		var result = await _userApiService.EditUserAsync(user.Id, user, token);
		if (result)
			return RedirectToAction("GoProfile");

		return View(user);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		var token = HttpContext.Session.GetString("JwtToken");
		var result = await _userApiService.DeleteUserAsync(id, token);
		return RedirectToAction("Index", "Home");
	}


    [HttpGet]
    public async Task<IActionResult> GoProfile()
    {
        var token = HttpContext.Session.GetString("JwtToken");
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var user = await _userApiService.GetUserById(userId, token);
        var orders = await _orderApiService.GetUserOrdersAsync(token);

        var viewModel = new UserProfileVM
        {
            User = user,
            Orders = orders
        };

        return View(viewModel);
    }
}

