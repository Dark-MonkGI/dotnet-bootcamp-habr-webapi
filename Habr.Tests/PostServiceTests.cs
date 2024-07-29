using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Services;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Habr.BusinessLogic.Profiles;

namespace Habr.BusinessLogic.Tests
{
    public class PostServiceTests
    {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly Mock<ILogger<PostService>> _loggerMock;

            public PostServiceTests()
            {
                _context = GetDataContext();
                _loggerMock = new Mock<ILogger<PostService>>();

                var config = new MapperConfiguration(cfg => cfg.AddProfile<PostProfile>());
                _mapper = config.CreateMapper();
            }

            private DataContext GetDataContext()
            {
                var options = new DbContextOptionsBuilder<DataContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;

                var context = new DataContext(options);
                context.Database.EnsureCreated();

                return context;
            }

            [Fact]
            public async Task CreatePost_Should_Create_Post_Successfully()
            {
                // Arrange
                var postService = new PostService(_context, _mapper, _loggerMock.Object);
                var createPostDto = new CreatePostDto
                {
                    UserId = 1,
                    Title = "Test Post",
                    Text = "This is a test post",
                    IsPublished = false
                };

                // Act
                var post = await postService.CreatePost(createPostDto);

                // Assert
                Assert.NotNull(post);
                Assert.Equal(createPostDto.Title, post.Title);
                Assert.Equal(createPostDto.Text, post.Text);
            }

        [Fact]
        public async Task CreatePost_Should_Fail_Without_Title()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var createPostDto = new CreatePostDto
            {
                UserId = 1,
                Title = null,
                Text = "This is a test post",
                IsPublished = false
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.CreatePost(createPostDto));
        }

        [Fact]
        public async Task CreatePost_Should_Fail_Without_Text()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var createPostDto = new CreatePostDto
            {
                UserId = 1,
                Title = "Test Post",
                Text = null,
                IsPublished = false
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.CreatePost(createPostDto));
        }

        [Fact]
        public async Task PublishPostAsync_Should_Publish_Post_Successfully()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = false,
                IsDeleted = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            await postService.PublishPostAsync(postId, userId);

            // Assert
            Assert.True(post.IsPublished);
            Assert.NotNull(post.PublishedDate);
        }

        [Fact]
        public async Task PublishPostAsync_Should_Fail_For_NonExistent_Post()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.PublishPostAsync(postId, userId));
        }

        [Fact]
        public async Task PublishPostAsync_Should_Fail_For_AlreadyPublished_Post()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = true,
                IsDeleted = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postService.PublishPostAsync(postId, userId));
        }

        [Fact]
        public async Task MovePostToDraftAsync_Should_Move_Post_To_Draft_Successfully()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = true,
                IsDeleted = false,
                Comments = new List<Comment>()
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            await postService.MovePostToDraftAsync(postId, userId);

            // Assert
            Assert.False(post.IsPublished);
            Assert.Null(post.PublishedDate);
        }

        [Fact]
        public async Task MovePostToDraftAsync_Should_Fail_For_NonExistent_Post()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.MovePostToDraftAsync(postId, userId));
        }

        [Fact]
        public async Task MovePostToDraftAsync_Should_Fail_For_AlreadyInDrafts_Post()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = false,
                IsDeleted = false,
                Comments = new List<Comment>()
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postService.MovePostToDraftAsync(postId, userId));
        }

        [Fact]
        public async Task MovePostToDraftAsync_Should_Fail_For_PostWithComments()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = true,
                IsDeleted = false,
                Comments = new List<Comment>
                {
                    new Comment { Id = 1, PostId = postId, UserId = userId, Text = "Test comment" }
                }
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => postService.MovePostToDraftAsync(postId, userId));
        }

        [Fact]
        public async Task DeletePost_Should_Delete_Post_Successfully()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;
            var post = new Post
            {
                Id = postId,
                UserId = userId,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = true,
                IsDeleted = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act
            await postService.DeletePost(postId, userId);

            // Assert
            Assert.True(post.IsDeleted);
        }

        [Fact]
        public async Task DeletePost_Should_Fail_For_NonExistent_Post()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.DeletePost(postId, userId));
        }

        [Fact]
        public async Task DeletePost_Should_Fail_For_NonAuthor_User()
        {
            // Arrange
            var postService = new PostService(_context, _mapper, _loggerMock.Object);
            var postId = 1;
            var userId = 2;
            var post = new Post
            {
                Id = postId,
                UserId = 1,
                Title = "Test Post",
                Text = "This is a test post",
                IsPublished = true,
                IsDeleted = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => postService.DeletePost(postId, userId));
        }
    }
}
