using ClickEatApp.Core.DTOs;

namespace ClickEatApp.Web.ViewModel.User
{
    public class UserProfileVM
    {
        public UserDto User { get; set; }
        public List<OrderDto> Orders { get; set; } = new();
    }
}
