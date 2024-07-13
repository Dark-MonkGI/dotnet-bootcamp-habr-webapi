using Habr.BusinessLogic.Resources;
using System.Text.RegularExpressions;
using Habr.Common;

namespace Habr.BusinessLogic.Validation
{
    public static class UserValidation
    {
        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(Messages.EmailEmpty);
            }

            if (email.Length > Constants.User.EmailMaxLength)
            {
                throw new ArgumentException(string.Format(Messages.EmailTooLong, Constants.User.EmailMaxLength));
            }

            var emailRegex = new Regex(Constants.User.EmailPattern);
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
