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
                var postService = new PostService(context);
                var commentService = new CommentService(context);
                User authenticatedUser;

                // Authorization
                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine("What do you want to do? Please enter:\n " +
                        "R - for register;\n " +
                        "A - for login;\n " +
                        "0 - to exit;");
                    Console.WriteLine(new string('-', 95) + "\n");

                    var userInput = Console.ReadLine()?.Trim().ToLower();

                    if (userInput == "r")
                    {
                        authenticatedUser = await UserManager.RegisterUser(userService);
                        if (authenticatedUser != null)
                        {
                            break;
                        }
                    }
                    else if (userInput == "a")
                    {
                        authenticatedUser = await UserManager.AuthenticateUser(userService);
                        if (authenticatedUser != null)
                        {
                            break;
                        }
                    }
                    else if (userInput == "0")
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid action. Please try again.");
                    }
                }

                // Logic for working with posts and comments
                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine("What do you want to do? Please enter:\n " +
                        "G - for get all posts;\n " +
                        "C - for create post;\n " +
                        "E - for edit post;\n " +
                        "D - for delete post;\n " +
                        "A - for add comment to post;\n " +
                        "R - for reply to comment;\n " +
                        "X - for delete comment;\n " +
                        "0 - to exit;");
                    Console.WriteLine(new string('-', 95) + "\n");

                    var userInput = Console.ReadLine()?.Trim().ToLower();
                    switch (userInput)
                    {
                        case "g":
                            await PostManager.DisplayAllPosts(postService);
                            break;
                        case "c":
                            await PostManager.CreatePost(postService, authenticatedUser);
                            break;
                        case "e":
                            await PostManager.EditPost(postService, authenticatedUser);
                            break;
                        case "d":
                            await PostManager.DeletePost(postService, authenticatedUser);
                            break;
                        case "a":
                            await CommentManager.AddComment(commentService, postService, authenticatedUser);
                            break;
                        case "r":
                            await CommentManager.AddReply(commentService, postService, authenticatedUser);
                            break;
                        case "x":
                            await CommentManager.DeleteComment(commentService, authenticatedUser);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid action. Please try again.");
                            break;
                    }
                }
            }
        }
    }
}
