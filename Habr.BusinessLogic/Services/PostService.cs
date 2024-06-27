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
                .Where(p => p.IsPublished && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedDate)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorEmail = p.User.Email,
                    PublicationDate = p.PublishedDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId)
        {
            return await _context.Posts
                .Where(p => 
                    !p.IsPublished && 
                    p.UserId == userId && 
                    !p.IsDeleted)
                .OrderByDescending(p => p.Updated)
                .Select(p => new DraftPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedAt = p.Created,
                    UpdatedAt = p.Updated
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllUserPosts(int userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted)
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
                .Where(p => 
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            return post;
        }

        public async Task UpdatePost(Post post)
        {
            var existingPost = await GetPostWithCommentsAsync(post.Id, post.UserId);

            if (existingPost == null)
            {
                throw new ArgumentException("Post not found.");
            }

            if (existingPost.IsPublished)
            {
                throw new InvalidOperationException("A published post cannot be edited. Move it to drafts first.");
            }

            existingPost.Title = post.Title;
            existingPost.Text = post.Text;
            existingPost.Updated = DateTime.UtcNow;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p => 
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("The post does not exist.");
            }

            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> PublishPostAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p => 
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsPublished)
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
                    p.Id == postId &&
                    p.UserId == userId &&
                    p.IsPublished)
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
