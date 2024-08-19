using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Services;
using Habr.DataAccess.Entities;

namespace Habr.Application.Controllers
{
    public class PostsController
    {
        private readonly PostService _postService;

        public PostsController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IEnumerable<PostDtoV1>> GetAllPostsAsync()
        {
            return await _postService.GetAllPublishedPosts(CancellationToken.None);
        }

        public async Task<IEnumerable<DraftPostDto>> GetUserDraftPostsAsync(int userId)
        {
            return await _postService.GetUserDraftPosts(userId, CancellationToken.None);
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(int userId)
        {
            return await _postService.GetAllUserPosts(userId, CancellationToken.None);
        }

        public async Task<Post> CreatePostAsync(CreatePostDto createPostDto)
        {
            return await _postService.CreatePost(createPostDto, CancellationToken.None);
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId)
        {
            return await _postService.GetPostWithCommentsAsync(postId, userId, CancellationToken.None);
        }

        public async Task UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            await _postService.UpdatePost(updatePostDto, CancellationToken.None);
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            await _postService.DeletePost(postId, userId, CancellationToken.None);
        }

        public async Task PublishPostAsync(int postId, int userId)
        {
            await _postService.PublishPostAsync(postId, userId, CancellationToken.None);
        }

        public async Task MovePostToDraftAsync(int postId, int userId)
        {
            await _postService.MovePostToDraftAsync(postId, userId, CancellationToken.None);
        }

        public async Task<PostDetailsDto> GetPostDetailsAsync(int postId)
        {
            return await _postService.GetPostDetailsAsync(postId, CancellationToken.None);
        }
    }
}
