using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Habr.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string 'DefaultConnection' is empty. Please check your settings!");
                return;
            }

            using (var context = new DataContext(connectionString))
            {
                await context.Database.MigrateAsync();

                var post = new Post
                {
                    Title = "First post",
                    Text = "Text describing the first post",
                    Created = DateTime.UtcNow
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
