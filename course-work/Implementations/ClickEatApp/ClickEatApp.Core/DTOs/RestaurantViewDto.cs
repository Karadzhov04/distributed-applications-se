using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickEatApp.Core.DTOs;

namespace ClickEatApp.Core.DTOs
{
    public class RestaurantViewDto
    {
        public int Id { get; set; } // За навигация/линкове

        public string Name { get; set; }

        public string ImageUrl { get; set; } // Подготвен път до снимката

        public string Description { get; set; }

        public bool IsOpen { get; set; }
        public string? OwnerName { get; set; }
        public int? OwnerId { get; set; }
        public List<FoodViewDto>? Foods { get; set; } // Само ако е нужно (пример: в детайли)
    }

}

