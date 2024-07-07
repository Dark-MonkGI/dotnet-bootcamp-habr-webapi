using Habr.DataAccess.Entities;
using Habr.Application.Controllers;
using Habr.ConsoleApp.Helpers;
using Habr.BusinessLogic.Validation;
using Habr.BusinessLogic.DTOs;
using Habr.ConsoleApp.Resources;

namespace Habr.ConsoleApp.Managers
{
    public static class PostManager
    {
        public static async Task DisplayAllPosts(PostsController postController, User authenticatedUser)
        {
            if (authenticatedUser == null)
            {
                Console.WriteLine(Messages.NeedToBeLogged);
                return;
            }

            var posts = await postController.GetAllPostsAsync();

            if (posts == null || !posts.Any())
            {
                Console.WriteLine(Messages.NoPostsFound);
                return;
            }

            DisplayHelper.DisplayPosts(posts);
        }

        public static async Task DisplayUserDraftPosts(PostsController postController, int userId)
        {
            var posts = await postController.GetUserDraftPostsAsync(userId);

            if (posts == null || !posts.Any())
            {
                Console.WriteLine(Messages.NoDraftPostsFound);
                return;
            }

            DisplayHelper.DisplayDraftPosts(posts);
        }

        public static async Task CreatePost(PostsController postController, User user)
        {
            var title = InputHelper.GetInputWithValidation(Messages.EnterPostTitle, PostValidation.ValidateTitle);
            if (title == null)
            {
                return;
            }

            var text = InputHelper.GetInputWithValidation(Messages.EnterPostText, PostValidation.ValidateText);
            if (text == null)
            {
                return;
            }

            var isPublishedInput = InputHelper.GetInputWithValidation(Messages.IsPostPublished, input =>
            {
                if (!input.Equals(Messages.Yes, StringComparison.OrdinalIgnoreCase) && !input.Equals(Messages.No, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(Messages.PleaseEnterYesOrNo);
                }
            });

            if (isPublishedInput == null)
            {
                return;
            }

            var isPublished = isPublishedInput.Equals(Messages.Yes, StringComparison.OrdinalIgnoreCase);

            try
            {
                var post = await postController.CreatePostAsync(new CreatePostDto
                {
                    Title = title,
                    Text = text,
                    IsPublished = isPublished
                }, 
                user.Id);

                Console.WriteLine(string.Format(Messages.PostCreatedSuccessfully, user.Name));
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

        public static async Task EditPost(PostsController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine(Messages.YouHaveNoPostsToEdit);
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToEdit, input =>
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

            try
            {
                var post = await postController.GetPostWithCommentsAsync(postId, user.Id);

                if (post == null)
                {
                    Console.WriteLine(Messages.PostNotFoundForYou);
                    return;
                }

                if (post.IsPublished)
                {
                    Console.WriteLine(Messages.PostIsPublishedCannotEdit);
                    return;
                }

                var title = InputHelper.GetInputWithValidation(Messages.EnterNewTitle, PostValidation.ValidateTitle);
                if (title == null)
                {
                    return;
                }

                var text = InputHelper.GetInputWithValidation(Messages.EnterNewText, PostValidation.ValidateText);
                if (text == null)
                {
                    return;
                }

                var updatePostDto = new UpdatePostDto
                {
                    Title = title,
                    Text = text
                };

                await postController.UpdatePostAsync(postId, updatePostDto, user.Id);

                Console.WriteLine(Messages.PostUpdated);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
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

        public static async Task DeletePost(PostsController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine(Messages.YouHaveNoPostsToDelete);
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToDelete, input =>
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

            try
            {
                await postController.DeletePostAsync(postId, user.Id);
                Console.WriteLine(Messages.PostDeleted);
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

        public static async Task PublishPost(PostsController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine(Messages.YouHaveNoPostsToPublish);
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToPublish, input =>
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

            try
            {
                await postController.PublishPostAsync(postId, user.Id);
                Console.WriteLine(Messages.PostPublished);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
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

        public static async Task MovePostToDraft(PostsController postController, User user)
        {
            var userPosts = await postController.GetUserPostsAsync(user.Id);

            if (userPosts == null || !userPosts.Any())
            {
                Console.WriteLine(Messages.YouHaveNoPostsToMoveToDrafts);
                return;
            }

            DisplayHelper.DisplayUserPosts(userPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostIDToMoveToDrafts, input =>
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

            try
            {
                await postController.MovePostToDraftAsync(postId, user.Id);
                Console.WriteLine(Messages.PostMovedToDraftsSuccessfully);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
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

        public static async Task DisplayPostDetails(PostsController postController)
        {
            var publishedPosts = await postController.GetAllPostsAsync();

            if (publishedPosts == null || !publishedPosts.Any())
            {
                Console.WriteLine(Messages.NoPublishedPostsFound);
                return;
            }

            DisplayHelper.DisplayPosts(publishedPosts);

            var postIdInput = InputHelper.GetInputWithValidation(Messages.EnterPostID, input =>
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

            try
            {
                var postDetails = await postController.GetPostDetailsAsync(postId);
                if (postDetails == null)
                {
                    Console.WriteLine(Messages.PostNotFound);
                    return;
                }

                DisplayHelper.DisplayPostDetails(postDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(Messages.Error, ex.Message));
            }
        }
    }
}
