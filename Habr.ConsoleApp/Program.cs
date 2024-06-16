using Habr.ConsoleApp.Managers;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Habr.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string 'DefaultConnection' is empty. Please check your settings!");
                return;
            }

            using (var context = new DataContext(connectionString))
            {
                await context.Database.MigrateAsync();

                var userService = new UserService(context);

                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine("What do you want to do? Please enter:\n R - for register;\n A - for login;\n 0 - to exit;");
                    Console.WriteLine(new string('-', 95) + "\n");

                    var action = Console.ReadLine()?.Trim().ToLower();

                    if (action == "r")
                    {
                        if (await UserManager.RegisterUser(userService))
                        {
                            return;
                        }
                    }
                    else if (action == "a")
                    {
                        if (await UserManager.AuthenticateUser(userService))
                        {
                            return;
                        }
                    }
                    else if (action == "0")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid action. Please try again.");
                    }
                }

            }
        }
    }
}
