using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;

namespace Habr.BusinessLogic.Services
{
    public class PostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllPublishedPosts()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.Created)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetUserDraftPosts(int userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => !p.IsPublished && p.UserId == userId)
                .OrderByDescending(p => p.Updated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllUserPosts(int userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Created)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Post> CreatePost(
            int userId, 
            string title, 
            string text, 
            bool isPublished)
        {
            var post = new Post
            {
                UserId = userId,
                Title = title,
                Text = text,
                IsPublished = isPublished,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId)
        {
            var post =  await _context.Posts
                .Include(p => p.Comments)
                .Where(p => p.Id == postId && p.UserId == userId)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return post;
        }

        public async Task UpdatePost(Post post)
        {
            var existingPost = await GetPostWithCommentsAsync(post.Id, post.UserId);

            if (existingPost != null)
            {
                existingPost.Title = post.Title;
                existingPost.Text = post.Text;
                existingPost.IsPublished = post.IsPublished;
                existingPost.Updated = DateTime.UtcNow;

                _context.Posts.Update(existingPost);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p => p.Id == postId && p.UserId == userId)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
