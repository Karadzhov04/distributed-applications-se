using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
    public class FoodEditDto
    {
        public int Id { get; set; }

        [Display(Name = "Име на храната")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Цена(лв.)")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MaxLength(300)]
        public string Description { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Required]
        [Display(Name = "Калории")]
        public int Calories { get; set; }

        public int RestaurantId { get; set; }
    }
}
