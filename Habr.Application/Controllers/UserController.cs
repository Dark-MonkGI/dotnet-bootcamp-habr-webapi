using Habr.BusinessLogic.Services;
using Habr.DataAccess.Entities;

namespace Habr.Application.Controllers
{
    public class UserController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<User> RegisterAsync(string email, string password, bool isEmailConfirmed)
        {
            return await _userService.RegisterAsync(email, password, isEmailConfirmed);
        }

        public async Task ConfirmEmailAsync(string email, bool isEmailConfirmed)
        {
            await _userService.ConfirmEmailAsync(email, isEmailConfirmed);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            return await _userService.AuthenticateAsync(email, password);
        }
    }
}
