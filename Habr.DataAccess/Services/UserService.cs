using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services
{
    public class UserService
    {
        private readonly DataContext context;

        public UserService(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Register(string name, string email, string password)
        {
            if (await context.Users.AnyAsync(u => u.Email == email))
            {
                throw new ArgumentException("A user with this email already exists.");
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Created = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }
    }
}
