using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Habr.BusinessLogic.Resources;
using Microsoft.Extensions.Logging;
using X.PagedList;
using X.Extensions.PagedList.EF;

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

        public async Task<IEnumerable<PostDtoV1>> GetAllPublishedPosts(CancellationToken cancellationToken)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedDate)
                .ProjectTo<PostDtoV1>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<PostDtoV2>> GetAllPublishedPostsV2(
            PaginatedParametersDto paginatedParametersDto, CancellationToken cancellationToken)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedDate)
                .ProjectTo<PostDtoV2>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(
                    paginatedParametersDto.PageNumber, 
                    paginatedParametersDto.PageSize, 
                    null, 
                    cancellationToken
                );
        }

        public async Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId, CancellationToken cancellationToken)
        {
            return await _context.Posts
                .Where(p =>
                    !p.IsPublished &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .OrderByDescending(p => p.Updated)
                .ProjectTo<DraftPostDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Post>> GetAllUserPosts(int userId, CancellationToken cancellationToken)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.Created)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Post> CreatePost(CreatePostDto createPostDto, CancellationToken cancellationToken)
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
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(string.Format(LogMessages.PostCreatedSuccessfully, post.Id, createPostDto.UserId));

            return post;
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId, CancellationToken cancellationToken)
        {
            var post =  await _context.Posts
                .Include(p => p.Comments)
                .Where(p => 
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            return post;
        }

        public async Task<Post> GetPostByIdAsync(int postId, int userId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            return post;
        }

        public async Task UpdatePost(UpdatePostDto updatePostDto, CancellationToken cancellationToken)
        {
            var existingPost = await GetPostByIdAsync(updatePostDto.PostId, updatePostDto.UserId, cancellationToken);

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
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePost(int postId, int userId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                throw new ArgumentException(Messages.PostDoesNotExist);
            }

            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task PublishPostAsync(int postId, int userId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

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
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(string.Format(LogMessages.PostPublishedSuccessfully, postId, userId));
        }

        public async Task MovePostToDraftAsync(int postId, int userId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

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
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<PostDetailsDto> GetPostDetailsAsync(int postId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Where(p => p.Id == postId && p.IsPublished && !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                throw new ArgumentException(Messages.PostNotFoundOrUnpublished);
            }

            return _mapper.Map<PostDetailsDto>(post);
        }

        public async Task UpdatePostAsAdmin(UpdatePostDto updatePostDto, CancellationToken cancellationToken)
        {
            var existingPost = await _context.Posts
                .Where(p => p.Id == updatePostDto.PostId && !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (existingPost == null)
            {
                throw new ArgumentException(Messages.PostNotFound);
            }

            var originalUserId = existingPost.UserId;

            _mapper.Map(updatePostDto, existingPost);
            existingPost.UserId = originalUserId;
            existingPost.Updated = DateTime.UtcNow;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePostAsAdmin(int postId, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p => p.Id == postId && !p.IsDeleted)
                .SingleOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                throw new ArgumentException(Messages.PostDoesNotExist);
            }

            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
