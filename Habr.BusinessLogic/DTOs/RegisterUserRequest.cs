﻿namespace Habr.BusinessLogic.DTOs
{
    public class RegisterUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}
