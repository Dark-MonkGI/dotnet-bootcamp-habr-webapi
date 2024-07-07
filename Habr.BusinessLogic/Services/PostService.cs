using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Habr.BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public PostService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PostDto>> GetAllPublishedPosts()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedDate)
                .ProjectTo<PostDto>(_mapper.ConfigurationProvider)
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
                .ProjectTo<DraftPostDto>(_mapper.ConfigurationProvider)
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

        public async Task<Post> CreatePost(CreatePostDto createPostDto, int userId)
        {
            var post = _mapper.Map<Post>(createPostDto);
            post.UserId = userId;
            post.Created = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;
            post.PublishedDate = createPostDto.IsPublished ? DateTime.UtcNow : (DateTime?)null;

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

        public async Task<Post> GetPostByIdAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return post;
        }

        public async Task UpdatePost(int postId, int userId, UpdatePostDto updatePostDto)
        {
            var existingPost = await GetPostByIdAsync(postId, userId);

            if (existingPost == null)
            {
                throw new ArgumentException("Post not found.");
            }

            if (existingPost.IsPublished)
            {
                throw new InvalidOperationException("A published post cannot be edited. Move it to drafts first.");
            }

            _mapper.Map(updatePostDto, existingPost);
            existingPost.Updated = DateTime.UtcNow;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePost(int postId, int userId)
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
        }

        public async Task PublishPostAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("\nThe post does not exist.");
            }

            if (post.IsPublished)
            {
                throw new InvalidOperationException("\nThe post is already published.");
            }

            post.IsPublished = true;
            post.PublishedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task MovePostToDraftAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("\nThe post does not exist.");
            }

            if (!post.IsPublished)
            {
                throw new InvalidOperationException("\nThe post is already in drafts.");
            }

            if (post.Comments.Any())
            {
                throw new InvalidOperationException("\nThe post cannot be moved to drafts because it has comments.");
            }

            post.IsPublished = false;
            post.PublishedDate = null;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PostDetailsDto> GetPostDetailsAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Where(p => p.Id == postId && p.IsPublished && !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("Post not found or is not published.");
            }

            return _mapper.Map<PostDetailsDto>(post);
        }
    }
}
