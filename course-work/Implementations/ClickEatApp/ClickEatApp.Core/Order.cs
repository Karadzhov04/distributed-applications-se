using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core
{
	public class Order
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public DateTime OrderDate { get; set; }

		public decimal TotalPrice { get; set; } 

		public bool Delivery { get; set; }

		[Required]
		[MaxLength(100)]
		public string Address { get; set; } = null!;

		public User User { get; set; }
		public List<OrderFood> OrderFoods { get; set; } = new List<OrderFood>();
	}
}
