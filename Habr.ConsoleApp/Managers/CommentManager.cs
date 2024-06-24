using Habr.DataAccess.Entities;
using Habr.ConsoleApp.Helpers;
using Habr.Application.Controllers;
using Habr.BusinessLogic.Validation;

namespace Habr.ConsoleApp.Managers
{
    public static class CommentManager
    {
        public static async Task AddComment(CommentController commentController, PostController postController, User user)
        {
            var publishedPosts = await postController.GetAllPostsAsync();

            if (publishedPosts == null || !publishedPosts.Any())
            {
                Console.WriteLine("No published posts found!");
                return;
            }

            DisplayHelper.DisplayPosts(publishedPosts);

            var postIdInput = InputHelper.GetInputWithValidation("Enter the ID of the post you want to comment on:", input =>
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

            var text = InputHelper.GetInputWithValidation("Enter your comment:", CommentValidation.ValidateCommentText);

            if (text == null)
            {
                return;
            }

            try
            {
                await commentController.AddCommentAsync(user.Id, postId, text);
                Console.WriteLine($"{user.Name}, your comment has been successfully added!");
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

        public static async Task AddReply(CommentController commentController, PostController postController, User user)
        {
            var publishedPosts = await postController.GetAllPostsAsync();

            if (publishedPosts == null || !publishedPosts.Any())
            {
                Console.WriteLine("No published posts found!");
                return;
            }

            DisplayHelper.DisplayPosts(publishedPosts);

            var postIdInput = InputHelper.GetInputWithValidation("Enter the ID of the post you want to see comments:", input =>
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
            var comments = await commentController.GetCommentsByPostAsync(postId);

            if (comments == null || !comments.Any())
            {
                Console.WriteLine("No comments found for this post!");
                return;
            }

            DisplayHelper.DisplayComments(comments);

            var commentIdInput = InputHelper.GetInputWithValidation("Enter the ID of the comment you want to reply to:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("Invalid ID format.");
                }
            });

            if (commentIdInput == null)
            {
                return;
            }

            var commentId = int.Parse(commentIdInput);

            var text = InputHelper.GetInputWithValidation("Enter your reply:", CommentValidation.ValidateCommentText);

            if (text == null)
            {
                return;
            }

            try
            {
                await commentController.AddReplyAsync(user.Id, commentId, text);
                Console.WriteLine($"{user.Name}, your reply has been successfully added!");
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

        public static async Task DeleteComment(CommentController commentController, User user)
        {
            var userComments = await commentController.GetUserCommentsAsync(user.Id);

            if (!userComments.Any())
            {
                Console.WriteLine("You have no comments to delete");
                return;
            }

            DisplayHelper.DisplayComments(userComments);

            var commentIdInput = InputHelper.GetInputWithValidation("Enter the ID of the comment you want to delete:", input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException("Invalid ID format.");
                }
            });

            if (commentIdInput == null)
            {
                return;
            }

            var commentId = int.Parse(commentIdInput);

            try
            {
                var deleted = await commentController.DeleteCommentAsync(commentId, user.Id);
                if (deleted)
                {
                    Console.WriteLine("Comment deleted!");
                }
                else
                {
                    Console.WriteLine("Comment not found or you do not have permission to delete it!");
                }
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