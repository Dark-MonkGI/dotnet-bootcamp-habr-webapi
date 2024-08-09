using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using Habr.ConsoleApp.Resources;

namespace Habr.ConsoleApp.Helpers
{
    public static class DisplayHelper
    {
        public static void DisplayPosts(IEnumerable<PostDtoV1> posts)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{1, -40} | {2, -30} | {3, -20}", Messages.Title, Messages.AuthorEmail, Messages.DatePublication);
            Console.WriteLine(new string('-', 115));

            foreach (var post in posts)
            {
                Console.WriteLine("{1, -40} | {2, -30} | {3, -20}", post.Title, post.AuthorEmail, post.PublishedAt);
            }

            Console.WriteLine(new string('-', 115));
        }

        public static void DisplayDraftPosts(IEnumerable<DraftPostDto> posts)
        {
            Console.WriteLine("\n" + new string('-', 95));
            Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20}", Messages.Id, Messages.Title, Messages.DateCreation, Messages.DateLastUpdate);
            Console.WriteLine(new string('-', 95));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20}", post.Id, post.Title, post.CreatedAt, 
                    post.UpdatedAt.HasValue ? post.UpdatedAt.Value.ToString() : "");
            }

            Console.WriteLine(new string('-', 95));
        }

        public static void DisplayUserPosts(IEnumerable<Post> posts)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20} | {4, -30}", Messages.Id, Messages.Title, Messages.DateCreation,
                Messages.DateLastUpdate, Messages.IsPublished);
            Console.WriteLine(new string('-', 115));

            foreach (var post in posts)
            {
                Console.WriteLine("{0, -5} | {1, -30} | {2, -20} | {3, -20} | {4, -30}", post.Id, post.Title, 
                    post.Created, post.Updated, post.IsPublished ? Messages.Yes : Messages.No);
            }

            Console.WriteLine(new string('-', 115));
        }

        public static void DisplayComments(IEnumerable<Comment> comments)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine("{0, -5} | {1, -25} | {2, -20} | {3, -10} | {4, -15} | {5, -20}", Messages.Id, Messages.PostId, Messages.DateCreation,
                Messages.PostId, Messages.UserName, Messages.IdParentComment);
            Console.WriteLine(new string('-', 115));

            foreach (var comment in comments)
            {
                Console.WriteLine("{0, -5} | {1, -25} | {2, -20} | {3, -10} | {4, -15} | {5, -20}",
                    comment.Id, comment.Text, comment.Created, comment.PostId, comment.User.Name, comment.ParentCommentId);
            }

            Console.WriteLine(new string('-', 115) + "\n");
        }

        public static void DisplayPostDetails(PostDetailsDto post)
        {
            Console.WriteLine("\n" + new string('-', 115));
            Console.WriteLine(string.Format(Messages.TitleWithColon, post.Title));
            Console.WriteLine(string.Format(Messages.TextWithColon, post.Text)); 
            Console.WriteLine(string.Format(Messages.AuthorEmailWithColon, post.AuthorEmail)); 
            Console.WriteLine(string.Format(Messages.DatePublicationWithColon, post.PublicationDate)); 
            Console.WriteLine(new string('-', 115));
            Console.WriteLine(Messages.CommentsWithColon);

            foreach (var comment in post.Comments)
            {
                Console.WriteLine(string.Format(Messages.TableFields, comment.Id, comment.UserName, comment.Created, comment.Text, comment.ParentCommentId));
            }

            Console.WriteLine(new string('-', 115));
        }
    }
}
