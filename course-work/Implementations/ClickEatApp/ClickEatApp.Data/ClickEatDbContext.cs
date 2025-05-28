using ClickEatApp.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Data
{
	public class ClickEatDbContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Food> Foods { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Restaurant> Restaurant { get; set; }
		public DbSet<OrderFood> OrderFoods { get; set; }

		public ClickEatDbContext(DbContextOptions<ClickEatDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region User Config
			modelBuilder.Entity<User>()
				.HasKey(u => u.Id);

			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					Name = "Ivan Karadzhov",
					Email = "ivan@abv.bg",
					Password = "378873",
					DateOfBirth = new DateTime(2000, 1, 1),
					Gender = 0,
					Role = 0
				});
			#endregion

			#region OrderFood Config
			modelBuilder.Entity<OrderFood>()
			.HasKey(of => new { of.OrderId, of.FoodId });

			// Order - OrderFood (1:N)
			modelBuilder.Entity<OrderFood>()
				.HasOne(of => of.Order)
				.WithMany(o => o.OrderFoods)
				.HasForeignKey(of => of.OrderId);

			// Food - OrderFood (1:N)
			modelBuilder.Entity<OrderFood>()
				.HasOne(of => of.Food)
				.WithMany(f => f.OrderFoods)
				.HasForeignKey(of => of.FoodId);
			#endregion

			#region Food Config
			modelBuilder.Entity<Food>()
				.HasKey(f => f.Id);
			#endregion

			#region Order Config
			modelBuilder.Entity<Order>()
				.HasKey(o => o.Id);

			//  User - Order (1:N)
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserId);
			#endregion

			modelBuilder.Entity<Food>()
				.Property(f => f.Price)
				.HasPrecision(18, 2);

			modelBuilder.Entity<Order>()
				.Property(o => o.TotalPrice)
				.HasPrecision(18, 2);

			modelBuilder.Entity<OrderFood>()
				.Property(of => of.PriceAtTime)
				.HasPrecision(18, 2);

            modelBuilder.Entity<Restaurant>()
				.HasOne(r => r.Owner)
				.WithMany()
				.HasForeignKey(r => r.OwnerId);

            modelBuilder.Entity<Food>()
                .HasOne(f => f.Restaurant)
                .WithMany(r => r.Foods)
                .HasForeignKey(f => f.RestaurantId);

            modelBuilder.Entity<Food>()
			   .HasOne(f => f.Restaurant)
			   .WithMany(r => r.Foods)
			   .HasForeignKey(f => f.RestaurantId)
			   .OnDelete(DeleteBehavior.Restrict);
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=DESKTOP-RU0BL0N\SQLEXPRESS;Database=ClickEatDb;Trusted_Connection=True;TrustServerCertificate=True;");
		}
	}
}
