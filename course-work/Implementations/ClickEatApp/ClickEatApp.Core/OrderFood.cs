using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core
{
	public class OrderFood
	{
		public int OrderId { get; set; }

		public int FoodId { get; set; }

		[Required]
		public int Quantity { get; set; }

		public decimal PriceAtTime { get; set; }

		public Order Order { get; set; }
		public Food Food { get; set; } 
	}
}
