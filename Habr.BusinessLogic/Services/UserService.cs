using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.DataAccess;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.DTOs;
using AutoMapper;
using Habr.BusinessLogic.Resources;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(DataContext context, IMapper mapper, ILogger<UserService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<User> RegisterAsync(RegisterUserRequest registerUserDto)
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

        public async Task<User> AuthenticateAsync(AuthenticateUserRequest authenticateUserDto)
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
