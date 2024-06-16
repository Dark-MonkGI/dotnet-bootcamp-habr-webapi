using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services
{
    public class PostService
    {
        private readonly DataContext context;

        public DataContext GetContext()
        {
            return context;
        }

        public PostService(DataContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Post>> GetAllPosts()
        {
            return await context.Posts.Include(p => p.User).ToListAsync();
        }

        public async Task<Post> CreatePost(int userId, string title, string text)
        {
            var post = new Post
            {
                UserId = userId,
                Title = title,
                Text = text,
                Created = DateTime.UtcNow
            };

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> UpdatePost(int postId, string title, string text)
        {
            var post = await context.Posts.FindAsync(postId);
            if (post == null)
            {
                return null;
            }

            post.Title = title;
            post.Text = text;
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<bool> DeletePost(int postId)
        {
            var post = await context.Posts.FindAsync(postId);
            if (post == null)
            {
                return false;
            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
