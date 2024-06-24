using System.Text.RegularExpressions;

namespace Habr.BusinessLogic.Validation
{
    public static class UserValidation
    {
        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            if (email.Length > 200)
            {
                throw new ArgumentException("Email cannot exceed 200 characters.");
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                throw new ArgumentException("Email format is invalid.");
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            if (password.Length < 1)
            {
                throw new ArgumentException("Password must be at least 1 characters long.");
            }
        }
    }
}
