using Habr.DataAccess.Entities;
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

        public static async Task<User> RegisterUser(UserService userService)
        {
            var name = GetInputWithExitOption("Enter your name");
            if (name == null) 
            {
                return null;
            }

            var email = GetInputWithExitOption("Enter your email");
            if (email == null)
            {
                return null;
            }

            var password = GetInputWithExitOption("Enter your password");
            if (password == null)
            {
                return null;
            }

            try
            {
                var user = await userService.Register(name, email, password);
                Console.WriteLine($"\nRegistration successful. Welcome, {user.Name}!");
                return user;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }

            return null;
        }

        public static async Task<User> AuthenticateUser(UserService userService)
        {
            while (true)
            {
                var email = GetInputWithExitOption("Enter your email");
                if (email == null)
                {
                    return null;
                }

                var password = GetInputWithExitOption("Enter your password");
                if (password == null)
                {
                    return null;
                }

                try
                {
                    var user = await userService.Authenticate(email, password);
                    if (user != null)
                    {
                        Console.WriteLine($"\nAuthentication successful. Welcome back, {user.Name}!");
                        return user;
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

                return null;
            }
        }
    }
}
