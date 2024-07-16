using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Habr.BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Habr.Application.Controllers;
using Habr.ConsoleApp.Managers;
using AutoMapper;
using Habr.BusinessLogic.Profiles;
using Habr.ConsoleApp.Resources;
using Microsoft.Extensions.Logging;
using Serilog;


namespace Habr.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog(Log.Logger);
            });

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine(Messages.DefaultConnectionEmpty);
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


                var userService = new UserService(context, mapper, loggerFactory.CreateLogger<UserService>());
                var postService = new PostService(context, mapper, loggerFactory.CreateLogger<PostService>());
                var commentService = new CommentService(context, mapper);

                var usersController = new UsersController(userService);
                var postsController = new PostsController(postService);
                var commentsController = new CommentsController(commentService);

                User authenticatedUser;

                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine(Messages.ActionSelection);
                    Console.WriteLine(Messages.Register);
                    Console.WriteLine(Messages.Confirm);
                    Console.WriteLine(Messages.Login);
                    Console.WriteLine(Messages.ToExit);
                    Console.WriteLine(new string('-', 95) + "\n");

                    var userInput = Console.ReadLine()?.Trim().ToLower();

                    if (userInput == Messages.R)
                    {
                        authenticatedUser = await UserManager.RegisterUser(usersController);
                        if (authenticatedUser != null)
                        {
                            Console.WriteLine(string.Format(Messages.RegistrationSuccessful, authenticatedUser.Name));
                            if (authenticatedUser.IsEmailConfirmed)
                            {
                                break;
                            }
                            else
                            {
                                Console.WriteLine(Messages.DonNotForgetConfirmEmail);
                            }
                        }
                    }
                    else if (userInput == Messages.C)
                    {
                        authenticatedUser = await UserManager.ConfirmEmail(usersController);
                        if (authenticatedUser != null)
                        {
                            break;
                        }
                    }
                    else if (userInput == Messages.A)
                    {
                        authenticatedUser = await UserManager.AuthenticateUser(usersController);
                        if (authenticatedUser != null)
                        {
                            Console.WriteLine(string.Format(Messages.AuthorizationWasSuccessful, authenticatedUser.Name));
                            break;
                        }
                    }
                    else if (userInput == Messages.Zero)
                    {
                        return;
                    }
                    else
                    {
                        Console.WriteLine(Messages.InvalidAction);
                    }
                }
                
                while (true)
                {
                    Console.WriteLine("\n" + new string('-', 95));
                    Console.WriteLine(Messages.WhatDoYouWant);
                    Console.WriteLine(Messages.GetAllPosts);
                    Console.WriteLine(Messages.ViewDraftPosts);
                    Console.WriteLine(Messages.CreatePost);
                    Console.WriteLine(Messages.EditPost);
                    Console.WriteLine(Messages.DeletePost);
                    Console.WriteLine(Messages.PublishPost);
                    Console.WriteLine(Messages.MovePostToDrafts);
                    Console.WriteLine(Messages.PostDetails);
                    Console.WriteLine(Messages.CommentToPost);
                    Console.WriteLine(Messages.ReplyToComment);
                    Console.WriteLine(Messages.DeleteComment);
                    Console.WriteLine(Messages.DisplayUserName);
                    Console.WriteLine(Messages.ToExit);
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
                        case "n":
                            await UserManager.GetAuthenticatedUserNameAsync(authenticatedUser);
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine(Messages.InvalidAction);
                            break;
                    }
                }
            }
        }
    }
}
