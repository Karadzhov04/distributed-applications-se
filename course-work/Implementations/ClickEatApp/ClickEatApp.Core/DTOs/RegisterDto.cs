using ClickEatApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
	public class RegisterDto
	{
        [Required]
        [Display(Name = "Потребителско име")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Имейл")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Парола")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Дата на раждане")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Пол")]
        public GenderEnum Gender { get; set; }
	}
}
