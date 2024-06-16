using System;
using System.Threading.Tasks;
using Habr.DataAccess.Services;

namespace Habr.ConsoleApp.Managers
{
    public static class UserManager
    {
        private static string GetInputWithExitOption(string heading)
        {
            while (true)
            {
                Console.WriteLine($"\n{heading} (or 0 to exit):");
                var input = Console.ReadLine()?.Trim();

                if (input == "0")
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }

                Console.WriteLine("Input cannot be empty. Please try again.");
            }
        }

        public static async Task<bool> RegisterUser(UserService userService)
        {
            var name = GetInputWithExitOption("Enter your name");
            if (name == null) 
            {
                return false;
            }

            var email = GetInputWithExitOption("Enter your email");
            if (email == null)
            {
                return false;
            }

            var password = GetInputWithExitOption("Enter your password");
            if (password == null)
            {
                return false;
            }

            try
            {
                var user = await userService.Register(name, email, password);
                Console.WriteLine($"\nRegistration successful. Welcome, {user.Name}!");
                return true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            return false;
        }

        public static async Task<bool> AuthenticateUser(UserService userService)
        {
            while (true)
            {
                var email = GetInputWithExitOption("Enter your email");
                if (email == null)
                {
                    return false;
                }

                var password = GetInputWithExitOption("Enter your password");
                if (password == null)
                {
                    return false;
                }

                try
                {
                    var user = await userService.Authenticate(email, password);
                    if (user != null)
                    {
                        Console.WriteLine($"\nAuthentication successful. Welcome back, {user.Name}!");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("\nAuthentication failed. Please check your email and password or enter 0 to exit.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                }

                return false;
            }
        }
    }
}
