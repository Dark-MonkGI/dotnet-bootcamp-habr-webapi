using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using System.Security.Claims;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(RegisterUserDto registerUserDto, ClaimsPrincipal user);
        Task<(string Token, string Message)> ConfirmEmailAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);
        Task<(string Token, string Message)> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);

        Task<User> RegisterAsync(RegisterUserDto registerUserDto);
        Task ConfirmEmailAsync(string email, bool isEmailConfirmed);
        Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto);
    }
}
