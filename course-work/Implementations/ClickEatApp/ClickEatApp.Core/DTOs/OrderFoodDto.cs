using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
	public class OrderFoodDto
	{
		public int FoodId { get; set; }
		public string Name { get; set; }
		public decimal PriceAtTime { get; set; }
		public int Quantity { get; set; }
	}
}
