using ClickEatApp.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Data
{
    public class RestaurantRepository : IRepository<Restaurant>
    {
        private readonly ClickEatDbContext _context;

        public RestaurantRepository(ClickEatDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurant> GetByIdAsync(int id)
        {
            return await _context.Restaurant.FindAsync(id);
        }

        public async Task<IEnumerable<Restaurant>> GetAllAsync()
        {
            return await _context.Restaurant
                .Include(r => r.Owner) // <-- Зарежда Owner като навигационно свойство
                .ToListAsync();
        }

        public async Task AddAsync(Restaurant entity)
        {
            await _context.Restaurant.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Restaurant entity)
        {
            _context.Restaurant.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var food = await _context.Restaurant.FindAsync(id);
            if (food != null)
            {
                _context.Restaurant.Remove(food);
                await _context.SaveChangesAsync();
            }
        }
    }
}
