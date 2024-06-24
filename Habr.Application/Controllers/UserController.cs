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

        public async Task<User> RegisterAsync(string email, string password)
        {
            return await _userService.RegisterAsync(email, password);
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            return await _userService.AuthenticateAsync(email, password);
        }
    }
}
