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
using Microsoft.AspNetCore.Identity;
using Habr.Common;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

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
            ITokenService tokenService,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _roleManager = roleManager;
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
            newUser.UserName = registerUserDto.Email.Split('@')[0];
            newUser.Created = DateTime.UtcNow;
            newUser.RefreshToken = _tokenService.GenerateRefreshToken();
            newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);
            newUser.SecurityStamp = Guid.NewGuid().ToString();

            await _userManager.CreateAsync(newUser, registerUserDto.Password);
            await _userManager.AddToRoleAsync(newUser, Constants.Roles.User);

            var roles = await _userManager.GetRolesAsync(newUser);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Email, newUser.Email)
            }.Concat(roleClaims));

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

            var authenticatedUser = await _userManager.FindByEmailAsync(authenticateUserDto.Email);

            if (authenticatedUser == null)
            {
                throw new ArgumentException(Messages.InvalidEmail);
            }

            if (!authenticateUserDto.IsEmailConfirmed)
            {
                throw new ArgumentException(Messages.EmailConfirmationFailed);
            }

            authenticatedUser.IsEmailConfirmed = true;
            authenticatedUser.RefreshToken = _tokenService.GenerateRefreshToken();
            authenticatedUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            await _userManager.UpdateAsync(authenticatedUser);

            var roles = await _userManager.GetRolesAsync(authenticatedUser);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Email, authenticatedUser.Email)
            }.Concat(roleClaims));

            return new TokenResponseDto { 
                Token = token, 
                RefreshToken = authenticatedUser.RefreshToken, 
                Message = Messages.EmailConfirmedSuccessfully 
            };
        }

        public async Task<TokenResponseDto> AuthenticateUserAsync(AuthenticateUserDto authenticateUserDto, ClaimsPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException(Messages.AlreadyAuthenticated);
            }

            var authenticatedUser = await _userManager.FindByEmailAsync(authenticateUserDto.Email);

            if (authenticatedUser == null)
            {
                throw new ArgumentException(Messages.InvalidEmailOrPassword);
            }

            if (!authenticatedUser.IsEmailConfirmed)
            {
                return new TokenResponseDto { Message = Messages.ConfirmYourEmail };
            }

            var roles = await _userManager.GetRolesAsync(authenticatedUser);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var token = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
                new Claim(ClaimTypes.Email, authenticatedUser.Email)
            }.Concat(roleClaims));

            var refreshToken = _tokenService.GenerateRefreshToken();
            authenticatedUser.RefreshToken = refreshToken;
            authenticatedUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

            await _userManager.UpdateAsync(authenticatedUser);

            return new TokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Message = Messages.AuthenticationSuccessful
            };
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
            var currentUtcTime = DateTime.UtcNow;

            if (user == null || user.RefreshTokenExpiryTime <= currentUtcTime)
            {
                throw new UnauthorizedAccessException(Messages.InvalidRefreshToken);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var newAccessToken = _tokenService.GenerateAccessToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }.Concat(roleClaims));

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = currentUtcTime.AddDays(_jwtSettings.RefreshTokenLifetimeDays);

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
