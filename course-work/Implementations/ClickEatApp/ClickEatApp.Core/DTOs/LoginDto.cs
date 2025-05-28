using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
	public class LoginDto
	{
        [Required]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required]
        [Display(Name = "Парола")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
	}
}
