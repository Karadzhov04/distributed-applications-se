using ClickEatApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
	public class EditDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = null!;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		public DateTime DateOfBirth { get; set; }

		public GenderEnum Gender { get; set; }
	}
}
