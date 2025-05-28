using ClickEatApp.Core.Enums;

namespace ClickEatApp.Core.DTOs
{
	public class UserDto
	{
		public int Id { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public DateTime DateOfBirth { get; set; }
		public GenderEnum Gender { get; set; }
	}

}

