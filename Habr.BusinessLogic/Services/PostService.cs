using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Resources;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(DataContext context, IMapper mapper, ILogger<PostService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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

        public async Task<Post> CreatePost(CreatePostDto createPostDto)
        {
            if (string.IsNullOrWhiteSpace(createPostDto.Title))
            {
                throw new ArgumentException(Messages.TitleRequired);
            }

            if (string.IsNullOrWhiteSpace(createPostDto.Text))
            {
                throw new ArgumentException(Messages.TextRequired);
            }

            var post = _mapper.Map<Post>(createPostDto);
            post.Created = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;
            post.PublishedDate = createPostDto.IsPublished ? DateTime.UtcNow : (DateTime?)null;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(LogMessages.PostCreatedSuccessfully, post.Id, createPostDto.UserId));

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

        public async Task UpdatePost(UpdatePostDto updatePostDto)
        {
            var existingPost = await GetPostByIdAsync(updatePostDto.PostId, updatePostDto.UserId);

            if (existingPost == null)
            {
                throw new ArgumentException(Messages.PostNotFound);
            }

            if (existingPost.IsPublished)
            {
                throw new InvalidOperationException(Messages.PostPublishedError);
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
                throw new ArgumentException(Messages.PostDoesNotExist);
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
                throw new ArgumentException(Messages.PostDoesNotExist);
            }

            if (post.IsPublished)
            {
                throw new InvalidOperationException(Messages.PostAlreadyPublished);
            }

            post.IsPublished = true;
            post.PublishedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            _logger.LogInformation(string.Format(LogMessages.PostPublishedSuccessfully, postId, userId));
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
                throw new ArgumentException(Messages.PostDoesNotExist);
            }

            if (!post.IsPublished)
            {
                throw new InvalidOperationException(Messages.PostInDrafts);
            }

            if (post.Comments.Any())
            {
                throw new InvalidOperationException(Messages.PostCommentsExist);
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
                throw new ArgumentException(Messages.PostNotFoundOrUnpublished);
            }

            return _mapper.Map<PostDetailsDto>(post);
        }
    }
}
