using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Services;
using Habr.DataAccess.Entities;

namespace Habr.Application.Controllers
{
    public class UsersController
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<User> RegisterAsync(RegisterUserDto registerUserDto)
        {
            return await _userService.RegisterAsync(registerUserDto);
        }

        public async Task ConfirmEmailAsync(string email, bool isEmailConfirmed)
        {
            await _userService.ConfirmEmailAsync(email, isEmailConfirmed);
        }

        public async Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto)
        {
            return await _userService.AuthenticateAsync(authenticateUserDto);
        }
    }
}
