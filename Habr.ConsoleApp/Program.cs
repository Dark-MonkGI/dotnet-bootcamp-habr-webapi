using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Habr.Application.Controllers;
using Habr.ConsoleApp.Managers;

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

                var userController = new UserController(userService);
                var postController = new PostController(postService);
                var commentController = new CommentController(commentService);

                User authenticatedUser;

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
                        authenticatedUser = await UserManager.RegisterUser(userController);
                        if (authenticatedUser != null)
                        {
                            Console.WriteLine($"\nRegistration successful. Welcome, {authenticatedUser.Name}!");
                            break;
                        }
                    }
                    else if (userInput == "a")
                    {
                        authenticatedUser = await UserManager.AuthenticateUser(userController);
                        if (authenticatedUser != null)
                        {
                            Console.WriteLine($"\nAuthorization was successful. Welcome back, {authenticatedUser.Name}!");
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

                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine("What do you want to do? Please enter:\n " +
                        "G - for get all posts;\n " +
                        "S - for view draft posts;\n " +
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
                            await PostManager.DisplayAllPosts(postController, authenticatedUser);
                            break;
                        case "s":
                            await PostManager.DisplayUserDraftPosts(postController, authenticatedUser.Id);
                            break;
                        case "c":
                            await PostManager.CreatePost(postController, authenticatedUser);
                            break;
                        case "e":
                            await PostManager.EditPost(postController, authenticatedUser);
                            break;
                        case "d":
                            await PostManager.DeletePost(postController, authenticatedUser);
                            break;
                        case "a":
                            await CommentManager.AddComment(commentController, postController, authenticatedUser);
                            break;
                        case "r":
                            await CommentManager.AddReply(commentController, postController, authenticatedUser);
                            break;
                        case "x":
                            await CommentManager.DeleteComment(commentController, authenticatedUser);
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
