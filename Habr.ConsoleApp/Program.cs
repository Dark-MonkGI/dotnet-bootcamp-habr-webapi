using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var context = new DataContext())
            {
                await context.Database.MigrateAsync();

                var post = new Post
                {
                    Title = "First post",
                    Text = "Text describing the first post",
                    Created = DateTime.Now
                };

                context.Posts.Add(post);
                await context.SaveChangesAsync();

                var posts = context.Posts.ToList();

                Console.WriteLine("\n" + new string('-', 95));
                Console.WriteLine("{0, -5} | {1, -20} | {2, -40} | {3, -20}", "Id", "Title", "Text", "Created");
                Console.WriteLine(new string('-', 95)); 

                foreach (var p in posts)
                {
                    Console.WriteLine("{0, -5} | {1, -20} | {2, -40} | {3, -20}", p.Id, p.Title, p.Text, p.Created);
                }
            }
        }
    }
}
