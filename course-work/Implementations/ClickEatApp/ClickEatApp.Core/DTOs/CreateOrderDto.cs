using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
	public class CreateOrderDto
	{
		public string UserEmail { get; set; } = null!;
		public List<CreateOrderItemDto> Items { get; set; } = new();
	}

	public class CreateOrderItemDto
	{
		public int FoodId { get; set; }
		public int Quantity { get; set; }
	}
}
