using ClickEatApp.Core;
using ClickEatApp.Core.DTOs;
using ClickEatApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Services
{
    public class OrderService
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly ClickEatDbContext _context;
        public OrderService(IRepository<Order> orderRepository, ClickEatDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			return await _orderRepository.GetAllAsync();
		}

		public async Task<Order> GetOrderByIdAsync(int id)
		{
			return await _orderRepository.GetByIdAsync(id);
		}

		public async Task UpdateOrderAsync(Order order)
		{
			await _orderRepository.UpdateAsync(order);
		}

		public async Task DeleteOrderAsync(int id)
		{
			await _orderRepository.DeleteAsync(id);
		}

        public async Task<int> CreateOrderAsync(OrderCreateDto dto)
        {
            var order = new Order
            {
                UserId = dto.UserId,
                Address = dto.Address,
                Delivery = dto.Delivery,
                OrderDate = dto.OrderDate,
                TotalPrice = dto.TotalPrice,
                OrderFoods = dto.Foods.Select(f => new OrderFood
                {
                    FoodId = f.FoodId,
                    Quantity = f.Quantity,
                    PriceAtTime = f.PriceAtTime
                }).ToList()
            };

           await _orderRepository.AddAsync(order);

            return order.Id;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderFoods)
                .ThenInclude(of => of.Food)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}
