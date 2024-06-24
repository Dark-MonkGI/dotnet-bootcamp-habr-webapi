using Habr.DataAccess.Entities;

namespace Habr.ConsoleApp.Helpers
{
    public static class DisplayHelper
    {
        public static void DisplayPosts(IEnumerable<Post> posts)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -40} | {2, -30} | {3, -20}", "Id", "Title", "Author's Email", "Date and Time of Publication");
            Console.WriteLine(new string('-', 115));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -40} | {2, -30} | {3, -20}", post.Id, post.Title, post.User.Email, post.Created);
            }

            Console.WriteLine(new string('-', 115));
        }

        public static void DisplayDraftPosts(IEnumerable<Post> posts)
        {
            Console.WriteLine("\n" + new string('-', 95));
            Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20}", "Id", "Title", "Date of Creation", "Date of Last Update");
            Console.WriteLine(new string('-', 95));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20}", post.Id, post.Title, post.Created, post.Updated);
            }

            Console.WriteLine(new string('-', 95));
        }

        public static void DisplayUserPosts(IEnumerable<Post> posts)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20} | {4, -30}", "Id", "Title", "Date of Creation", "Date of Last Update", "Is Published");
            Console.WriteLine(new string('-', 115));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20} | {4, -30}", post.Id, post.Title, post.Created, post.Updated, post.IsPublished ? "Yes" : "No");
            }

            Console.WriteLine(new string('-', 115));
        }

        public static void DisplayComments(IEnumerable<Comment> comments)
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
