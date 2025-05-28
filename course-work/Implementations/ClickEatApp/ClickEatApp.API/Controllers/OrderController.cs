using ClickEatApp.Core;
using ClickEatApp.Core.DTOs;
using ClickEatApp.Data;
using ClickEatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClickEatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IRepository<User> _userRepository;
        public OrdersController(OrderService orderService, IRepository<User> userRepository)
        {
            _orderService = orderService;
            _userRepository = userRepository;
        }

        // GET: api/Orders?name=pizza&minCalories=100&sortBy=price&page=1&pageSize=10
        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<OrderListResultDto>> GetAll(
                                                [FromQuery] string? clientName,
                                                [FromQuery] DateTime? orderDate,
                                                [FromQuery] int? minPrice,
                                                [FromQuery] string? sortBy = "id",
                                                [FromQuery] string? sortDirection = "asc",
                                                [FromQuery] int page = 1,
                                                [FromQuery] int pageSize = 10)
        {
            var orders = await _orderService.GetAllOrdersAsync();

            // Филтриране по име на клиент
            if (!string.IsNullOrEmpty(clientName))
            {
                var users = await _userRepository.GetAllAsync();
                var user = users.FirstOrDefault(u => u.Name == clientName);
                if (user != null)
                {
                    orders = orders.Where(o => o.UserId == user.Id);
                }
            }

            var query = orders.AsQueryable();

            // Филтриране по дата
            if (orderDate.HasValue)
            {
                query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
            }

            // Филтриране по минимална цена
            if (minPrice.HasValue)
            {
                query = query.Where(o => o.TotalPrice >= minPrice.Value);
            }

            // Сортиране
            query = (sortBy?.ToLower(), sortDirection?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(o => o.TotalPrice),
                ("price", _) => query.OrderBy(o => o.TotalPrice),
                ("id", "desc") => query.OrderByDescending(o => o.Id),
                ("id", _) => query.OrderBy(o => o.Id),
                _ => query.OrderBy(o => o.Id),
            };

            var total = query.Count();

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    Address = o.Address,
                    Delivery = o.Delivery,
                    TotalPrice = o.TotalPrice,
                    OrderDate = o.OrderDate,
                    Foods = o.OrderFoods.Select(of => new OrderFoodDto
                    {
                        FoodId = of.FoodId,
                        Quantity = of.Quantity,
                        PriceAtTime = of.PriceAtTime,
                        Name = of.Food.Name,
                    }).ToList()
                })
                .ToList();

            return Ok(new OrderListResultDto
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            try
            {
                int orderId = await _orderService.CreateOrderAsync(dto);
                return Ok(new { orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("UserOrders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Address = o.Address,
                Delivery = o.Delivery,
                TotalPrice = o.TotalPrice,
                Foods = o.OrderFoods.Select(f => new OrderFoodDto
                {
                    FoodId = f.FoodId,
                    Name = f.Food.Name,
                    Quantity = f.Quantity,
                    PriceAtTime = f.PriceAtTime
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}

