using ClickEatApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ClickEatApp.Data
{
	public class OrderRepository : IRepository<Order>
	{
		private readonly ClickEatDbContext _context;

		public OrderRepository(ClickEatDbContext context)
		{
			_context = context;
		}

		public async Task<Order> GetByIdAsync(int id)
		{
			return await _context.Orders.FindAsync(id);
		}

		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _context.Orders.ToListAsync();
		}

		public async Task AddAsync(Order entity)
		{
			await _context.Orders.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Order entity)
		{
			_context.Orders.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var order = await _context.Orders.FindAsync(id);
			if (order != null)
			{
				_context.Orders.Remove(order);
				await _context.SaveChangesAsync();
			}
		}
	}
}
