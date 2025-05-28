using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core
{
	public class Food
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = null!;

		public decimal Price { get; set; }

		[Required]
		[MaxLength(250)]
		public string Description { get; set; } = null!;

		public int Calories { get; set; }

		public DateTime CreatedOn { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<OrderFood> OrderFoods { get; set; } = new List<OrderFood>();

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

    }
}
