using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserDto registerUserDto);
        Task ConfirmEmailAsync(string email, bool isEmailConfirmed);
        Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto);
    }
}
