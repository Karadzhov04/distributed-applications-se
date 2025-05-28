using ClickEatApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ClickEatApp.Data
{
	public class FoodRepository : IFoodRepository
	{
		private readonly ClickEatDbContext _context;

		public FoodRepository(ClickEatDbContext context)
		{
			_context = context;
		}

		public async Task<Food> GetByIdAsync(int id)
		{
			return await _context.Foods.FindAsync(id);
		}
        public async Task<List<Food>> GetFoodsByRestaurantIdAsync(int restaurantId)
        {
            return await _context.Foods
                .Where(f => f.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Food>> GetAllAsync()
		{
			return await _context.Foods.ToListAsync();
		}

		public async Task AddAsync(Food entity)
		{
			await _context.Foods.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Food entity)
		{
			_context.Foods.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var food = await _context.Foods.FindAsync(id);
			if (food != null)
			{
				_context.Foods.Remove(food);
				await _context.SaveChangesAsync();
			}
		}
	}
}
