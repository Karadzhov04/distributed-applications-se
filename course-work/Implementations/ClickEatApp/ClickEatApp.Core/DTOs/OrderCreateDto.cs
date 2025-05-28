using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickEatApp.Core.DTOs
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public string Address { get; set; }
        public bool Delivery { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderFoodDto> Foods { get; set; } = new();
    }
}
