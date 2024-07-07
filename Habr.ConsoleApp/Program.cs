﻿using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Habr.Application.Controllers;
using Habr.ConsoleApp.Managers;
using AutoMapper;
using Habr.BusinessLogic.Profiles;

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

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<UserProfile>();
                    cfg.AddProfile<PostProfile>();
                    cfg.AddProfile<CommentProfile>();
                });

                var mapper = mapperConfig.CreateMapper();

                var userService = new UserService(context, mapper);
                var postService = new PostService(context, mapper);
                var commentService = new CommentService(context, mapper);

                var usersController = new UsersController(userService);
                var postsController = new PostsController(postService);
                var commentsController = new CommentsController(commentService);

                User authenticatedUser;

                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine("What do you want to do? Please enter:\n " +
                        "R - for register;\n " +
                        "C - for confirm email;\n " +
                        "A - for login;\n " +
                        "0 - to exit;");
                    Console.WriteLine(new string('-', 95) + "\n");

                    var userInput = Console.ReadLine()?.Trim().ToLower();

                    if (userInput == "r")
                    {
                        authenticatedUser = await UserManager.RegisterUser(usersController);
                        if (authenticatedUser != null)
                        {
                            Console.WriteLine($"\nRegistration successful. Welcome, {authenticatedUser.Name}!");
                            if (authenticatedUser.IsEmailConfirmed)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine("\nDon't forget to confirm your email!");
                            }
                        }
                    }
                    else if (userInput == "c")
                    {
                        authenticatedUser = await UserManager.ConfirmEmail(usersController);
                        if (authenticatedUser != null)
                        {
                            break;
                        }
                    }
                    else if (userInput == "a")
                    {
                        authenticatedUser = await UserManager.AuthenticateUser(usersController);
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
                        "P - for publish post;\n " +
                        "M - for move post to drafts;\n " +
                        "J - for post details;\n " +
                        "A - for add comment to post;\n " +
                        "R - for reply to comment;\n " +
                        "X - for delete comment;\n " +
                        "0 - to exit;");
                    Console.WriteLine(new string('-', 95) + "\n");

                    var userInput = Console.ReadLine()?.Trim().ToLower();
                    switch (userInput)
                    {
                        case "g":
                            await PostManager.DisplayAllPosts(postsController, authenticatedUser);
                            break;
                        case "s":
                            await PostManager.DisplayUserDraftPosts(postsController, authenticatedUser.Id);
                            break;
                        case "c":
                            await PostManager.CreatePost(postsController, authenticatedUser);
                            break;
                        case "e":
                            await PostManager.EditPost(postsController, authenticatedUser);
                            break;
                        case "d":
                            await PostManager.DeletePost(postsController, authenticatedUser);
                            break;
                        case "p":
                            await PostManager.PublishPost(postsController, authenticatedUser);
                            break;
                        case "m":
                            await PostManager.MovePostToDraft(postsController, authenticatedUser);
                            break;
                        case "j":
                            await PostManager.DisplayPostDetails(postsController);
                            break;
                        case "a":
                            await CommentManager.AddComment(commentsController, postsController, authenticatedUser);
                            break;
                        case "r":
                            await CommentManager.AddReply(commentsController, postsController, authenticatedUser);
                            break;
                        case "x":
                            await CommentManager.DeleteComment(commentsController, authenticatedUser);
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
