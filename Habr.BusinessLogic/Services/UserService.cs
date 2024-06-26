using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.DataAccess;

namespace Habr.BusinessLogic.Services
{
    public class UserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string email, string password)
        {
            UserValidation.ValidateEmail(email);
            UserValidation.ValidatePassword(password);

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ArgumentException("The email is already taken.");
            }

            var name = email.Split('@')[0];

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Created = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            UserValidation.ValidateEmail(email);
            UserValidation.ValidatePassword(password);

            var user = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new ArgumentException("The email is incorrect."); 
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid password.");
            }

            return user;
        }
    }
}
