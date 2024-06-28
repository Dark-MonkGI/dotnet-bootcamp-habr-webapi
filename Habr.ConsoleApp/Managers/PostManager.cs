using Habr.DataAccess.Entities;
using Habr.Application.Controllers;
using Habr.ConsoleApp.Helpers;
using Habr.BusinessLogic.Validation;

namespace Habr.ConsoleApp.Managers
{
    public static class PostManager
    {
        public static async Task DisplayAllPosts(PostController postController, User authenticatedUser)
        {
            if (authenticatedUser == null)
            {
                Console.WriteLine("You need to be logged in to view posts.");
                return;
            }

            var posts = await postController.GetAllPostsAsync();

            if (posts == null || !posts.Any())
            {
                Console.WriteLine("No posts found!");
                return;
            }

            DisplayHelper.DisplayPosts(posts);
        }

        public static async Task DisplayUserDraftPosts(PostController postController, int userId)
        {
            var posts = await postController.GetUserDraftPostsAsync(userId);

            if (posts == null || !posts.Any())
            {
                Console.WriteLine("No draft posts found!");
                return;
            }

            DisplayHelper.DisplayDraftPosts(posts);
        }

        public static async Task CreatePost(PostController postController, User user)
        {
            var title = InputHelper.GetInputWithValidation("Enter post title:", PostValidation.ValidateTitle);
            if (title == null)
            {
                return;
            }

            var text = InputHelper.GetInputWithValidation("Enter post text:", PostValidation.ValidateText);
            if (text == null)
            {
                return;
            }

            var isPublishedInput = InputHelper.GetInputWithValidation("Is the post published? (yes/no):", input =>
            {
                if (!input.Equals("yes", StringComparison.OrdinalIgnoreCase) && !input.Equals("no", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Please enter 'yes' or 'no'.");
                }
            });

            if (isPublishedInput == null)
            {
                return;
            }

            var isPublished = isPublishedInput.Equals("yes", StringComparison.OrdinalIgnoreCase);

            try
            {
                var post = await postController.CreatePostAsync(
                    user.Id, 
                    title, 
                    text, 
                    isPublished);
                Console.WriteLine($"{user.Name}, your post has been successfully created!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        public static async Task EditPost(PostController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine("\nYou have no posts to edit");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("\nEnter the ID of the post you want to edit:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("\nInvalid ID format.");
                }
            });

            if (postIdInput == null)
            {
                return;
            }

            var postId = int.Parse(postIdInput);

            try
            {
                var post = await postController.GetPostWithCommentsAsync(postId, user.Id);

                if (post == null)
                {
                    Console.WriteLine("\nThis post was not found for you!");
                    return;
                }

                if (post.IsPublished)
                {
                    Console.WriteLine("\nThis post is published and cannot be edited. Move it to drafts first.");
                    return;
                }

                var title = InputHelper.GetInputWithValidation("Enter new title:", PostValidation.ValidateTitle);
                if (title == null)
                {
                    return;
                }

                var text = InputHelper.GetInputWithValidation("Enter new text:", PostValidation.ValidateText);
                if (text == null)
                {
                    return;
                }

                post.Title = title;
                post.Text = text;
                post.Updated = DateTime.UtcNow;

                await postController.UpdatePostAsync(post);

                Console.WriteLine("\nPost updated!");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        public static async Task DeletePost(PostController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine("\nYou have no posts to delete");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("\nEnter the ID of the post you want to delete:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("Invalid ID format.");
                }
            });

            if (postIdInput == null)
            {
                return;
            }

            var postId = int.Parse(postIdInput);

            try
            {
                await postController.DeletePostAsync(postId, user.Id);
                Console.WriteLine("\nPost deleted!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        public static async Task PublishPost(PostController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine("\nYou have no posts to publish");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("\nEnter the ID of the post you want to publish:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("\nInvalid ID format.");
                }
            });

            if (postIdInput == null)
            {
                return;
            }

            var postId = int.Parse(postIdInput);

            try
            {
                await postController.PublishPostAsync(postId, user.Id);
                Console.WriteLine("\nPost published!");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\n{ex.Message}"); 
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }

        public static async Task MovePostToDraft(PostController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine("\nYou have no posts to move to drafts");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("\nEnter the ID of the post you want to move to drafts:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("\nInvalid ID format.");
                }
            });

            if (postIdInput == null)
            {
                return;
            }

            var postId = int.Parse(postIdInput);

            try
            {
                await postController.MovePostToDraftAsync(postId, user.Id);
                Console.WriteLine("\nPost moved to drafts successfully!");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }
}
