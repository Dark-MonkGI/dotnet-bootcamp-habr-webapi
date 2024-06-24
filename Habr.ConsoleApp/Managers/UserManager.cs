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

            try
            {
                return await userController.RegisterAsync(email, password);
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
