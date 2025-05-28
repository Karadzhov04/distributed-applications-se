using ClickEatApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ClickEatApp.Data
{
	public class UserRepository : IRepository<User>
	{
		private readonly ClickEatDbContext _context;

		public UserRepository(ClickEatDbContext context)
		{
			_context = context;
		}

		public async Task<User> GetByIdAsync(int id)
		{
			return await _context.Users.FindAsync(id);
		}

		public async Task<IEnumerable<User>> GetAllAsync()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task AddAsync(User entity)
		{
			await _context.Users.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(User entity)
		{
			_context.Users.Update(entity);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user != null)
			{
				_context.Users.Remove(user);
				await _context.SaveChangesAsync();
			}
		}
	}
}
