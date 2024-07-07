using Habr.BusinessLogic.Resources;
using System.Text.RegularExpressions;

namespace Habr.BusinessLogic.Validation
{
    public static class UserValidation
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const int MaxEmailLength = 200;

        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(Messages.EmailEmpty);
            }

            if (email.Length > MaxEmailLength)
            {
                throw new ArgumentException(string.Format(Messages.EmailTooLong, MaxEmailLength));
            }

            var emailRegex = new Regex(EmailPattern);
            if (!emailRegex.IsMatch(email))
            {
                throw new ArgumentException(Messages.EmailInvalidFormat);
            }
        }

        public static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(Messages.PasswordEmpty);
            }

            if (password.Length < 1)
            {
                throw new ArgumentException(Messages.PasswordTooShort);
            }
        }
    }
}
