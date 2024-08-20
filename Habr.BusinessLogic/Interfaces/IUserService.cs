using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using System.Security.Claims;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<TokenResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto, ClaimsPrincipal user, CancellationToken cancellationToken = default);
        Task<TokenResponseDto> ConfirmEmailAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);
        Task<TokenResponseDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<User> RegisterAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);
        Task ConfirmEmailAsync(string email, bool isEmailConfirmed, CancellationToken cancellationToken = default);
        Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto, CancellationToken cancellationToken = default);
    }
}
