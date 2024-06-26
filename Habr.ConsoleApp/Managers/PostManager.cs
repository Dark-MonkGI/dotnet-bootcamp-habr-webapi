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
                Console.WriteLine("You have no posts to edit");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("Enter the ID of the post you want to edit:", input =>
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

            var post = await postController.GetPostWithCommentsAsync(postId, user.Id);

            if (post == null)
            {
                Console.WriteLine("This post was not found for you!");
                return;
            }

            if (post.IsPublished && post.Comments.Any())
            {
                Console.WriteLine("This post cannot be edited because it has comments.");
                return;
            }

            if (post.IsPublished)
            {
                post.IsPublished = false;
                await postController.UpdatePostAsync(post);
                Console.WriteLine("The post has been moved to drafts. You can now edit it.");
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
                post.Title = title;
                post.Text = text;
                post.IsPublished = isPublished;
                post.Updated = DateTime.UtcNow;

                if (isPublished)
                {
                    post.PublishedDate = DateTime.UtcNow;
                }

                await postController.UpdatePostAsync(post);

                Console.WriteLine("Post updated!");
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
                Console.WriteLine("You have no posts to delete");
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation("Enter the ID of the post you want to delete:", input =>
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
                var postDeleted = await postController.DeletePostAsync(postId, user.Id);
                if (postDeleted)
                {
                    Console.WriteLine("Post deleted!");
                }
                else
                {
                    Console.WriteLine("The post does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
    }
}
