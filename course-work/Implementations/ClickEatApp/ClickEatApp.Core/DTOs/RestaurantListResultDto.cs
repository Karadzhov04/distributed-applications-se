using ClickEatApp.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
    public class RestaurantListResultDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<RestaurantViewDto> Items { get; set; } = new();
    }
}