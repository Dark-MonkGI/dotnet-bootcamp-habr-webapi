using Habr.DataAccess.Entities;
using Habr.ConsoleApp.Helpers;
using Habr.Application.Controllers;
using Habr.BusinessLogic.Validation;
using Habr.BusinessLogic.DTOs;
using Habr.ConsoleApp.Resources;

namespace Habr.ConsoleApp.Managers
{
    public static class CommentManager
    {
        public static async Task AddComment(CommentsController commentController, PostsController postController, User user)
        {
            var publishedPosts = await postController.GetAllPostsAsync();

            if (publishedPosts == null || !publishedPosts.Any())
            {
                Console.WriteLine(Messages.NoPublishedPostsFound);
                return;
            }

            DisplayHelper.DisplayPosts(publishedPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToComment, input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException(Messages.InvalidIDFormat);
                }
            });

            if (postIdInput == null)
            {
                return;
            }

            var postId = int.Parse(postIdInput);

            var text = InputHelper.GetInputWithValidation(Messages.EnterYourComment, CommentValidation.ValidateCommentText);

            if (text == null)
            {
                return;
            }

            try
            {
                await commentController.AddCommentAsync(new InternalAddCommentDto
                {
                    UserId = user.Id,
                    PostId = postId,
                    Text = text
                });

                Console.WriteLine(string.Format(Messages.CommentAddedSuccessfully, user.Name));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
            }
        }

        public static async Task AddReply(CommentsController commentController, PostsController postController, User user)
        {
            var publishedPosts = await postController.GetAllPostsAsync();

            if (publishedPosts == null || !publishedPosts.Any())
            {
                Console.WriteLine(Messages.NoPublishedPostsFound);
                return;
            }

            DisplayHelper.DisplayPosts(publishedPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToSeeComments, input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException(Messages.InvalidIDFormat);
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
                Console.WriteLine(Messages.NoCommentsFoundForPost);
                return;
            }

            DisplayHelper.DisplayComments(comments);

            var commentIdInput = InputHelper.GetInputWithValidation(Messages.EnterCommentIDToReply, input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException(Messages.InvalidIDFormat);
                }
            });

            if (commentIdInput == null)
            {
                return;
            }

            var commentId = int.Parse(commentIdInput);

            var text = InputHelper.GetInputWithValidation(Messages.EnterYourReply, CommentValidation.ValidateCommentText);

            if (text == null)
            {
                return;
            }

            try
            {
                await commentController.AddReplyAsync(new InternalAddReplyDto
                {
                    UserId = user.Id,
                    ParentCommentId = commentId,
                    Text = text
                });

                Console.WriteLine(string.Format(Messages.ReplyAddedSuccessfully, user.Name));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
            }
        }

        public static async Task DeleteComment(CommentsController commentController, User user)
        {
            var userComments = await commentController.GetUserCommentsAsync(user.Id);

            if (!userComments.Any())
            {
                Console.WriteLine(Messages.YouHaveNoCommentsToDelete);
                return;
            }

            DisplayHelper.DisplayComments(userComments);

            var commentIdInput = InputHelper.GetInputWithValidation(Messages.EnterCommentIDToDelete, input =>
            {
                if (!int.TryParse(input, out _))
                {
                    throw new ArgumentException(Messages.InvalidIDFormat);
                }
            });

            if (commentIdInput == null)
            {
                return;
            }

            var commentId = int.Parse(commentIdInput);

            try
            {
                await commentController.DeleteCommentAsync(commentId, user.Id);
                Console.WriteLine(Messages.CommentDeleted);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
            }
        }
    }
}