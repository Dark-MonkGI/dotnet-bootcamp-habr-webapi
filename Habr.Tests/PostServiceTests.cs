using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Habr.Tests
{
    public class PostServiceTests
    {
        [Fact]
        public void Test1()
        {

        }

        private readonly IMapper _mapper;
        private readonly Mock<DataContext> _contextMock;
        private readonly Mock<ILogger<PostService>> _loggerMock;
        private readonly PostService _postService;

        public PostServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new Habr.BusinessLogic.Profiles.PostProfile()));
            _mapper = config.CreateMapper();

            _contextMock = new Mock<DataContext>(new DbContextOptions<DataContext>());
            _loggerMock = new Mock<ILogger<PostService>>();

            _postService = new PostService(_contextMock.Object, _mapper, _loggerMock.Object);
        }

        [Fact]
        public async Task CreatePost_Should_Create_New_Post()
        {
            // Arrange
            var createPostDto = new CreatePostDto
            {
                UserId = 1,
                Title = "Test Post",
                Text = "This is a test post.",
                IsPublished = true
            };

            // Act
            var post = await _postService.CreatePost(createPostDto);

            // Assert
            Assert.NotNull(post);
            Assert.Equal(createPostDto.Title, post.Title);
            Assert.Equal(createPostDto.Text, post.Text);
            Assert.Equal(createPostDto.UserId, post.UserId);
            Assert.Equal(createPostDto.IsPublished, post.IsPublished);
        }

        [Fact]
        public async Task PublishPost_Should_Set_IsPublished_True()
        {
            // Arrange
            var post = new Post
            {
                Id = 1,
                UserId = 1,
                Title = "Test Post",
                Text = "This is a test post.",
                IsPublished = false
            };

            _contextMock.Setup(x => x.Posts.FindAsync(1)).ReturnsAsync(post);

            // Act
            await _postService.PublishPostAsync(post.Id, post.UserId);

            // Assert
            Assert.True(post.IsPublished);
        }

        [Fact]
        public async Task MovePostToDraft_Should_Set_IsPublished_False()
        {
            // Arrange
            var post = new Post
            {
                Id = 1,
                UserId = 1,
                Title = "Test Post",
                Text = "This is a test post.",
                IsPublished = true
            };

            _contextMock.Setup(x => x.Posts.FindAsync(1)).ReturnsAsync(post);

            // Act
            await _postService.MovePostToDraftAsync(post.Id, post.UserId);

            // Assert
            Assert.False(post.IsPublished);
        }

        [Fact]
        public async Task DeletePost_Should_Set_IsDeleted_True()
        {
            // Arrange
            var post = new Post
            {
                Id = 1,
                UserId = 1,
                Title = "Test Post",
                Text = "This is a test post.",
                IsDeleted = false
            };

            _contextMock.Setup(x => x.Posts.FindAsync(1)).ReturnsAsync(post);

            // Act
            await _postService.DeletePost(post.Id, post.UserId);

            // Assert
            Assert.True(post.IsDeleted);
        }
    }
}
