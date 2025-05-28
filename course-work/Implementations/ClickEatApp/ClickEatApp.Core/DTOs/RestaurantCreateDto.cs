using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
    public class RestaurantCreateDto
    {
        public int Id { get; set; }

        [Display(Name = "Наименование на ресторанта")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Display(Name = "Моля, качете изображение на ресторанта!")]
        [Required]
        public IFormFile Image { get; set; }

        [Display(Name = "Описание")]
        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        [Display(Name = "Отворен ли е в момента ресторантът?")]
        [Required]
        public bool IsOpen { get; set; } = true;

        public int? OwnerId { get; set; }
    }
}
