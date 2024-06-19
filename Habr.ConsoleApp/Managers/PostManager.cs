using Habr.DataAccess.Entities;
using Habr.DataAccess.Services;

namespace Habr.ConsoleApp.Managers
{
    public static class PostManager
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

        public static async Task DisplayAllPosts(PostService postService)
        {
            var posts = await postService.GetAllPosts();

            if (posts == null || !posts.Any())
            {
                Console.WriteLine("No posts found!");
                return;
            }

            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -20} | {2, -40} | {3, -20} | {4, -40}", "Id", "Title", "Text", "Date of Creation", "Name of the user");
            Console.WriteLine(new string('-', 115));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -20} | {2, -40} | {3, -20} | {4, -20}", post.Id, post.Title, post.Text, post.Created, post.User.Name);
            }

            Console.WriteLine(new string('-', 115) + "\n");
        }

        public static async Task CreatePost(PostService postService, User user)
        {
            var title = GetInput("Enter post title:");
            var text = GetInput("Enter post text:");
            var isPublishedInput = GetInput("Is the post published? (yes/no):").Trim();
            var isPublished = isPublishedInput.Equals("yes", StringComparison.OrdinalIgnoreCase);


            var post = await postService.CreatePost(user.Id, title, text, isPublished);
            Console.WriteLine($"{user.Name}, your post has been successfully created!");
        }

        public static async Task EditPost(PostService postService, User user)
        {
            var userPosts = await postService.GetUserPosts(user.Id);

            if (!userPosts.Any())
            {
                Console.WriteLine("You have no posts to edit");
                return;
            }

            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("{0, -5} | {1, -30}", "Id", "Title");
            Console.WriteLine(new string('-', 40));

            foreach (var userPost in userPosts)
            {
                Console.WriteLine("{0, -5} | {1, -30}", userPost.Id, userPost.Title);
            }
            Console.WriteLine(new string('-', 40) + "\n");

            var postId = int.Parse(GetInput("Enter the ID of the post you want to edit:"));
            var title = GetInput("Enter new title:");
            var text = GetInput("Enter new text:");
            var isPublishedInput = GetInput("Is the post published? (yes/no):").Trim();
            var isPublished = isPublishedInput.Equals("yes", StringComparison.OrdinalIgnoreCase);

            var post = await postService.UpdatePost(postId, user.Id, title, text, isPublished);
            if (post != null)
            {
                Console.WriteLine("Post updated!");
            }
            else
            {
                Console.WriteLine("This post was not found for you!");
            }
        }

        public static async Task DeletePost(PostService postService, User user)
        {
            var userPosts = await postService.GetUserPosts(user.Id);

            if (!userPosts.Any())
            {
                Console.WriteLine("You have no posts to delete");
                return;
            }

            Console.WriteLine("\n" + new string('-', 40));
            Console.WriteLine("{0, -5} | {1, -30}", "Id", "Title");
            Console.WriteLine(new string('-', 40));

            foreach (var userPost in userPosts)
            {
                Console.WriteLine("{0, -5} | {1, -30}", userPost.Id, userPost.Title);
            }
            Console.WriteLine(new string('-', 40) + "\n");

            var postId = int.Parse(GetInput("Enter the ID of the post you want to delete:"));

            var postDel = await postService.DeletePost(postId, user.Id);
            if (postDel)
            {
                Console.WriteLine("Post deleted!");
            }
            else
            {
                Console.WriteLine("This post was not found for you!");
            }
        }
    }
}
