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
            return await context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished)
                .ToListAsync();
        }

        public async Task<Post> CreatePost(int userId, string title, string text, bool isPublished)
        {
            var post = new Post
            {
                UserId = userId,
                Title = title,
                Text = text,
                Created = DateTime.UtcNow,
                IsPublished = isPublished
            };

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> UpdatePost(int postId, int userId, string title, string text, bool isPublished)
        {
            var post = await context.Posts
                .Where(p => p.Id == postId && p.UserId == userId)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return null;
            }

            post.Title = title;
            post.Text = text;
            post.IsPublished = isPublished;
            await context.SaveChangesAsync();

            return post;
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await context.Posts
                .Where(p => p.Id == postId && p.UserId == userId)
                .FirstOrDefaultAsync();

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
