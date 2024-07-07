using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.ConsoleApp.Helpers;
using Habr.Application.Controllers;
using Habr.BusinessLogic.DTOs;
using Habr.ConsoleApp.Resources;

namespace Habr.ConsoleApp.Managers
{
    public static class UserManager
    {
        public static async Task<User> RegisterUser(UsersController userController)
        {
            var email = InputHelper.GetInputWithValidation(Messages.EnterEmail, UserValidation.ValidateEmail);
            if (email == null)
            {
                return null;
            }

            var password = InputHelper.GetInputWithValidation(Messages.EnterYourPassword, UserValidation.ValidatePassword);
            if (password == null)
            {
                return null;
            }

            var random = new Random();
            var randomNumber = random.Next(1, 11);
            Console.WriteLine(string.Format(Messages.EnterNumberToConfirmEmail, randomNumber));

            var input = Console.ReadLine();
            bool isEmailConfirmed = input == randomNumber.ToString();

            try
            {
                var registerUserDto = new RegisterUserDto
                {
                    Email = email,
                    Password = password,
                    IsEmailConfirmed = isEmailConfirmed
                };

                var user = await userController.RegisterAsync(registerUserDto);

                Console.WriteLine(isEmailConfirmed
                    ? Messages.EmailConfirmed
                    : Messages.EmailNotConfirmed);

                return user;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
                return null;
            }
        }

        public static async Task<User> ConfirmEmail(UsersController userController)
        {
            var email = InputHelper.GetInputWithValidation(Messages.EnterEmail, UserValidation.ValidateEmail);
            if (email == null)
            {
                Console.WriteLine(Messages.EmailConfirmationCancelled);
                return null;
            }

            var password = InputHelper.GetInputWithValidation(Messages.EnterYourPassword, UserValidation.ValidatePassword);
            if (password == null)
            {
                Console.WriteLine(Messages.EmailConfirmationCancelled);
                return null;
            }

            try
            {
                var authenticatedUser = await userController.AuthenticateAsync(new AuthenticateUserDto
                {
                    Email = email,
                    Password = password
                });

                if (authenticatedUser == null)
                {
                    return null;
                }

                var random = new Random();
                var randomNumber = random.Next(1, 11);
                Console.WriteLine(string.Format(Messages.EnterNumberToConfirmEmail, randomNumber));

                var input = Console.ReadLine();
                bool isEmailConfirmed = input == randomNumber.ToString();

                await userController.ConfirmEmailAsync(email, isEmailConfirmed);
                Console.WriteLine(isEmailConfirmed
                    ? Messages.EmailConfirmedSuccessfully
                    : Messages.EmailConfirmationFailed);

                return isEmailConfirmed ? authenticatedUser : null;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
                return null;
            }
        }

        public static async Task<User> AuthenticateUser(UsersController userController)
        {
            var email = InputHelper.GetInputWithValidation(Messages.EnterEmail, UserValidation.ValidateEmail);
            if (email == null)
            {
                return null;
            }

            var password = InputHelper.GetInputWithValidation(Messages.EnterYourPassword, UserValidation.ValidatePassword);
            if (password == null)
            {
                return null;
            }

            try
            {
                return await userController.AuthenticateAsync(new AuthenticateUserDto
                {
                    Email = email,
                    Password = password
                });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
                return null;
            }
        }
    }
}
