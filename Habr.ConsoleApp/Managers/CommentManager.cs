using Habr.DataAccess.Entities;
using Habr.DataAccess.Services;

namespace Habr.ConsoleApp.Managers
{
    public static class CommentManager
    {
        public static string GetInput(string heading)
        {
            while (true)
            {
                Console.WriteLine(heading);
                var input = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }

                Console.WriteLine("Input cannot be empty. Please try again!");
            }
        }

        public static async Task AddComment(CommentService commentService, PostService postService, User user)
        {
            await PostManager.DisplayAllPosts(postService);

            var postId = int.Parse(GetInput("Enter the ID of the post you want to comment on:"));
            var text = GetInput("Enter your comment:");

            var comment = await commentService.AddComment(user.Id, postId, text);
            Console.WriteLine($"{user.Name}, your comment has been successfully added!");
        }

        public static async Task AddReply(CommentService commentService, PostService postService, User user)
        {
            await PostManager.DisplayAllPosts(postService);

            var postId = int.Parse(GetInput("Enter the ID of the post you want to see comments:"));
            var comments = await commentService.GetCommentsByPost(postId);

            DisplayComments(comments);

            var commentId = int.Parse(GetInput("Enter the ID of the comment you want to reply to:"));
            var text = GetInput("Enter your reply:");

            var reply = await commentService.AddReply(user.Id, commentId, text);
            Console.WriteLine($"{user.Name}, your reply has been successfully added!");
        }

        public static async Task DeleteComment(CommentService commentService, User user)
        {
            var userComments = await commentService.GetUserComments(user.Id);

            if (!userComments.Any())
            {
                Console.WriteLine("You have no comments to delete");
                return;
            }

            DisplayComments(userComments);

            var commentId = int.Parse(GetInput("Enter the ID of the comment you want to delete:"));

            var deleted = await commentService.DeleteComment(commentId, user.Id);
            if (deleted)
            {
                Console.WriteLine("Comment deleted!");
            }
            else
            {
                Console.WriteLine("Comment not found or you do not have permission to delete it!");
            }
        }

        private static void DisplayComments(IEnumerable<Comment> comments)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -25} | {2, -20} | {3, -10} | {4, -15} | {5, -20}", "Id", "Text", "Date of Creation", 
                "Post Id", "User Name", "ID parent comment");
            Console.WriteLine(new string('-', 115));

            foreach (var comment in comments)
            {
                Console.WriteLine("{0, -5} | {1, -25} | {2, -20} | {3, -10} | {4, -15} | {5, -20}", 
                    comment.Id, comment.Text, comment.Created, comment.PostId, comment.User.Name, comment.ParentCommentId);
            }

            Console.WriteLine(new string('-', 115) + "\n");
        }
    }
}