using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Services
{
    public class PostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDto>> GetAllPublishedPosts()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedDate)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorEmail = p.User.Email,
                    PublicationDate = p.PublishedDate
                })
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
                Updated = DateTime.UtcNow,
                PublishedDate = isPublished ? DateTime.UtcNow : (DateTime?)null
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

                if (!existingPost.IsPublished && post.IsPublished)
                {
                    existingPost.IsPublished = true;
                    existingPost.PublishedDate = DateTime.UtcNow;
                }
                else if (existingPost.IsPublished && !post.IsPublished)
                {
                    existingPost.IsPublished = false;
                    existingPost.PublishedDate = null;
                }

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

        public async Task<bool> PublishPostAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p => 
                    p.Id == postId 
                    && p.UserId == userId 
                    && !p.IsPublished)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            post.IsPublished = true;
            post.PublishedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MovePostToDraftAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Where(p => 
                    p.Id == postId 
                    && p.UserId == userId 
                    && p.IsPublished)
                .SingleOrDefaultAsync();

            if (post == null || !post.IsPublished || post.Comments.Any())
            {
                return false;
            }

            post.IsPublished = false;
            post.PublishedDate = null;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
