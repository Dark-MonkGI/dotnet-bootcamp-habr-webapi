using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Validation;
using Habr.ConsoleApp.Helpers;
using Habr.Application.Controllers;

namespace Habr.ConsoleApp.Managers
{
    public static class UserManager
    {
        public static async Task<User> RegisterUser(UserController userController)
        {
            var email = InputHelper.GetInputWithValidation("Enter your email", UserValidation.ValidateEmail);
            if (email == null)
            {
                return null;
            }

            var password = InputHelper.GetInputWithValidation("Enter your password", UserValidation.ValidatePassword);
            if (password == null)
            {
                return null;
            }

            var random = new Random();
            var randomNumber = random.Next(1, 11);
            Console.WriteLine($"Please enter the number {randomNumber} to confirm your email:");

            var input = Console.ReadLine();
            bool isEmailConfirmed = input == randomNumber.ToString();

            try
            {
                var user = await userController.RegisterAsync(email, password, isEmailConfirmed);

                Console.WriteLine(isEmailConfirmed
                    ? "\nEmail confirmed."
                    : "\nEmail not confirmed - please confirm your email later.");

                return user;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                return null;
            }
        }

        public static async Task<User> ConfirmEmail(UserController userController)
        {
            var email = InputHelper.GetInputWithValidation("Enter your email", UserValidation.ValidateEmail);
            if (email == null)
            {
                Console.WriteLine("\nEmail confirmation cancelled.");
                return null;
            }

            var password = InputHelper.GetInputWithValidation("Enter your password", UserValidation.ValidatePassword);
            if (password == null)
            {
                Console.WriteLine("\nEmail confirmation cancelled.");
                return null;
            }

            try
            {
                var authenticatedUser = await userController.AuthenticateAsync(email, password);
                if (authenticatedUser == null)
                {
                    return null;
                }

                var random = new Random();
                var randomNumber = random.Next(1, 11);
                Console.WriteLine($"\nPlease enter the number {randomNumber} to confirm your email:");

                var input = Console.ReadLine();
                bool isEmailConfirmed = input == randomNumber.ToString();

                await userController.ConfirmEmailAsync(email, isEmailConfirmed);
                Console.WriteLine(isEmailConfirmed
                    ? "\nEmail confirmed successfully."
                    : "\nEmail confirmation failed. Please try again.");

                return isEmailConfirmed ? authenticatedUser : null;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                return null;
            }
        }

        public static async Task<User> AuthenticateUser(UserController userController)
        {
            var email = InputHelper.GetInputWithValidation("Enter your email", UserValidation.ValidateEmail);
            if (email == null)
            {
                return null;
            }

            var password = InputHelper.GetInputWithValidation("Enter your password", UserValidation.ValidatePassword);
            if (password == null)
            {
                return null;
            }

            try
            {
                return await userController.AuthenticateAsync(email, password);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                return null;
            }
        }
    }
}
