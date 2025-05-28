using ClickEatApp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using ClickEatApp.Web.SessionExtensions;
using ClickEatApp.Web.Services;
using System.Security.Claims;

namespace ClickEatApp.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly FoodApiService _foodApiService;
        private readonly OrderApiService _orderApiService;

        public CartController(FoodApiService foodApiService, OrderApiService orderApiService)
        {
            _foodApiService = foodApiService;
            _orderApiService = orderApiService;
        }

        public IActionResult Index(int id)
        {
            ViewBag.RestaurantId = id;
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>("cart") ?? new List<CartItemDto>();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int foodId)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            var food = await _foodApiService.GetFoodByIdAsync(foodId, token);
            if (food == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItemDto>>("cart") ?? new List<CartItemDto>();

            var existingItem = cart.FirstOrDefault(x => x.FoodId == foodId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItemDto { FoodId = foodId, Name = food.Name, Price = food.Price, Quantity = 1 });
            }

            HttpContext.Session.SetObject("cart", cart);

            return RedirectToAction("Index", new { id = food.RestaurantId });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int foodId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>("cart") ?? new List<CartItemDto>();

            var item = cart.FirstOrDefault(x => x.FoodId == foodId);
            if (item != null) cart.Remove(item);

            HttpContext.Session.SetObject("cart", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string address, bool delivery)
        {
            var cart = HttpContext.Session.GetObject<List<CartItemDto>>("cart") ?? new List<CartItemDto>();

            var token = HttpContext.Session.GetString("JwtToken");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!cart.Any()) return RedirectToAction("Index");

            var dto = new OrderCreateDto
            {
                UserId = int.Parse(userId),
                Address = address,
                Delivery = delivery,
                OrderDate = DateTime.Now,
                TotalPrice = cart.Sum(x => x.Price * x.Quantity),
                Foods = cart.Select(x => new OrderFoodDto
                {
                    FoodId = x.FoodId,
                    Quantity = x.Quantity,
                    PriceAtTime = x.Price,
                    Name = x.Name,
                }).ToList()
            };

            await _orderApiService.CreateOrderAsync(dto, token);

            HttpContext.Session.Remove("cart");
            return RedirectToAction("ThankYou");
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
