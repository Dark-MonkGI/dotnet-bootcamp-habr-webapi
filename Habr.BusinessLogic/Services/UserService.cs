using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.DataAccess;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.DTOs;
using AutoMapper;
using Habr.BusinessLogic.Resources;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Habr.BusinessLogic.Helpers;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;

        public UserService(DataContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public UserService(
            DataContext context, 
            IMapper mapper, 
            ILogger<UserService> logger, 
            IOptions<JwtSettings> jwtSettings,
            ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _tokenService = tokenService;
        }

        public async Task<TokenResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto, ClaimsPrincipal user)
        {
            UserValidation.ValidateEmail(registerUserDto.Email);
            UserValidation.ValidatePassword(registerUserDto.Password);

            if (await _context.Users.AnyAsync(u => u.Email == registerUserDto.Email))
            {
                throw new ArgumentException(Messages.EmailTaken);
            }

            var newUser = _mapper.Map<User>(registerUserDto);
            newUser.Name = registerUserDto.Email.Split('@')[0];
            newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            newUser.Created = DateTime.UtcNow;
            newUser.RefreshToken = _tokenService.GenerateRefreshToken();
            newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Email, newUser.Email)
            });

            _logger.LogInformation(string.Format(LogMessages.UserRegisteredSuccessfully, newUser.Email));


            return new TokenResponseDto { 
                Token = token, 
                RefreshToken = newUser.RefreshToken, 
                Message = string.Format(Messages.UserRegisteredSuccessfully, newUser.Email)
            };
        }

        public async Task<TokenResponseDto> ConfirmEmailAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(Messages.AlreadyAuthenticated);
            }

            var authenticatedUser = await AuthenticateAsync(authenticateUserDto);

            if (authenticatedUser == null)
            {
                throw new ArgumentException(Messages.InvalidEmail);
            }

            if (!authenticateUserDto.IsEmailConfirmed)
            {
                throw new ArgumentException(Messages.EmailConfirmationFailed);
            }

            authenticatedUser.IsEmailConfirmed = true;
            _context.Users.Update(authenticatedUser);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Email, authenticatedUser.Email)
            });

            var refreshToken = _tokenService.GenerateRefreshToken();
            authenticatedUser.RefreshToken = refreshToken;
            authenticatedUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            _context.Users.Update(authenticatedUser);
            await _context.SaveChangesAsync();

            return new TokenResponseDto { 
                Token = token, 
                RefreshToken = refreshToken, 
                Message = Messages.EmailConfirmedSuccessfully 
            };
        }

        public async Task<TokenResponseDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(Messages.AlreadyAuthenticated);
            }

            var authenticatedUser = await AuthenticateAsync(authenticateUserDto);

            if (authenticatedUser == null)
            {
                throw new ArgumentException(Messages.InvalidEmailOrPassword);
            }

            if (!authenticatedUser.IsEmailConfirmed)
            {
                return new TokenResponseDto { Message = Messages.ConfirmYourEmail };
            }

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Email, authenticatedUser.Email)
            });

            var refreshToken = _tokenService.GenerateRefreshToken();
            authenticatedUser.RefreshToken = refreshToken;
            authenticatedUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            _context.Users.Update(authenticatedUser);
            await _context.SaveChangesAsync();

            return new TokenResponseDto { 
                Token = token, 
                RefreshToken = refreshToken, 
                Message = Messages.AuthenticationSuccessful 
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException(Messages.InvalidRefreshToken);
            }

            var newAccessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            });

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new TokenResponseDto { 
                Token = newAccessToken, 
                RefreshToken = newRefreshToken 
            };
        }

        public async Task<User> RegisterAsync(RegisterUserDto registerUserDto)
        {
            UserValidation.ValidateEmail(registerUserDto.Email);
            UserValidation.ValidatePassword(registerUserDto.Password);

            if (await _context.Users.AnyAsync(u => u.Email == registerUserDto.Email))
            {
                throw new ArgumentException(Messages.EmailTaken);
            }

            var user = _mapper.Map<User>(registerUserDto);
            user.Name = registerUserDto.Email.Split('@')[0];
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            user.Created = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(LogMessages.UserRegisteredSuccessfully, user.Email));

            return user;
        }

        public async Task ConfirmEmailAsync(string email, bool isEmailConfirmed)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new ArgumentException(Messages.EmailIncorrect);
            }

            user.IsEmailConfirmed = isEmailConfirmed;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(LogMessages.UserConfirmedSuccessfully, user.Email));
        }

        public async Task<User> AuthenticateAsync(AuthenticateUserDto authenticateUserDto)
        {
            UserValidation.ValidateEmail(authenticateUserDto.Email);
            UserValidation.ValidatePassword(authenticateUserDto.Password);

            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == authenticateUserDto.Email);

            if (user == null)
            {
                throw new ArgumentException(Messages.InvalidEmailOrPassword);
            }

            if (!BCrypt.Net.BCrypt.Verify(authenticateUserDto.Password, user.PasswordHash))
            {
                throw new ArgumentException(Messages.InvalidEmailOrPassword);
            }

            _logger.LogInformation(string.Format(LogMessages.UserLoggedInSuccessfully, user.Email));

            return user;
        }
    }
}
