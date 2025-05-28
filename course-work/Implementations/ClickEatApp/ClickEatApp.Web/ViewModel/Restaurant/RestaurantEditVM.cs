using ClickEatApp.Core.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClickEatApp.Web.ViewModel.Restaurant
{
    public class RestaurantEditVM
    {
        public RestaurantEditDto Restaurant { get; set; }

        public List<SelectListItem> Owners { get; set; }
    }
}
