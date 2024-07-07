using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.DataAccess;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.DTOs;
using AutoMapper;

namespace Habr.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<User> RegisterAsync(RegisterUserDto registerUserDto)
        {
            UserValidation.ValidateEmail(registerUserDto.Email);
            UserValidation.ValidatePassword(registerUserDto.Password);

            if (await _context.Users.AnyAsync(u => u.Email == registerUserDto.Email))
            {
                throw new ArgumentException("The email is already taken.");
            }

            var user = _mapper.Map<User>(registerUserDto);
            user.Name = registerUserDto.Email.Split('@')[0];
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password);
            user.Created = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task ConfirmEmailAsync(string email, bool isEmailConfirmed)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new ArgumentException("The email is incorrect.");
            }

            user.IsEmailConfirmed = isEmailConfirmed;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
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
                throw new ArgumentException("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(authenticateUserDto.Password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid email or password.");
            }

            return user;
        }
    }
}
