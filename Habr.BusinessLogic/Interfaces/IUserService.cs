using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using System.Security.Claims;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto, ClaimsPrincipal user);
        Task<TokenResponseDto> ConfirmEmailAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);
        Task<TokenResponseDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);

        Task<User> RegisterAsync(RegisterUserDto registerUserDto);
        Task ConfirmEmailAsync(string email, bool isEmailConfirmed);
        Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto);
    }
}
