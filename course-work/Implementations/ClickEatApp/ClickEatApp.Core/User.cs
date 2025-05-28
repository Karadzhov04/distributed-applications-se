using ClickEatApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClickEatApp.Core
{
	public class User
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = null!;

		[Required]
		[MaxLength(100)]
		public string Email { get; set; } = null!;

		[Required]
		[MaxLength(50)]
		public string Password { get; set; } = null!;

		public DateTime DateOfBirth { get; set; }

		public GenderEnum Gender { get; set; }

		public RoleEnum Role { get; set; } = RoleEnum.User;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
