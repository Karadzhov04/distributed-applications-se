using ClickEatApp.Core;
using ClickEatApp.Data;
using ClickEatApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace ClickEatApp.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// 🔽 1. Add DbContext (връзка с база данни)
			builder.Services.AddDbContext<ClickEatDbContext>(options =>
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

			// 🔽 2. Add Repositories
			builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<IFoodRepository, FoodRepository>();
            builder.Services.AddScoped<IRepository<Order>, OrderRepository>();
			builder.Services.AddScoped<IRepository<Restaurant>, RestaurantRepository>();


            // 🔽 3. Add Services
            builder.Services.AddScoped<OrderService>();

            // 🔽 4. Add Controllers and Swagger
            builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();


			builder.Services.AddAuthentication("Bearer")
				.AddJwtBearer("Bearer", options =>
				{
					var jwtSettings = builder.Configuration.GetSection("JwtSettings");
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtSettings["Issuer"],
						ValidAudience = jwtSettings["Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
						),


                        RoleClaimType = ClaimTypes.Role
                    };
				});

			builder.Services.AddAuthorization();

			var app = builder.Build();

			// HTTP pipeline
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}
            app.UseStaticFiles();

            app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}

