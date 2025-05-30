﻿using ClickEatApp.Core.Enums;

namespace ClickEatApp.Web.ViewModel.Admin
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
    }
}
