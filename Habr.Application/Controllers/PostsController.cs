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

        public async Task<IEnumerable<PostDto>> GetAllPostsAsync()
        {
            return await _postService.GetAllPublishedPosts();
        }

        public async Task<IEnumerable<DraftPostDto>> GetUserDraftPostsAsync(int userId)
        {
            return await _postService.GetUserDraftPosts(userId);
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(int userId)
        {
            return await _postService.GetAllUserPosts(userId);
        }

        public async Task<Post> CreatePostAsync(
            int userId, 
            string title, 
            string text, 
            bool isPublished)
        {
            return await _postService.CreatePost(
                userId, 
                title, 
                text, 
                isPublished);
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId)
        {
            return await _postService.GetPostWithCommentsAsync(postId, userId);
        }

        public async Task UpdatePostAsync(Post post)
        {
            await _postService.UpdatePost(post);
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            await _postService.DeletePost(postId, userId);
        }

        public async Task PublishPostAsync(int postId, int userId)
        {
            await _postService.PublishPostAsync(postId, userId);
        }

        public async Task MovePostToDraftAsync(int postId, int userId)
        {
            await _postService.MovePostToDraftAsync(postId, userId);
        }

        public async Task<PostDetailsDto> GetPostDetailsAsync(int postId)
        {
            return await _postService.GetPostDetailsAsync(postId);
        }
    }
}
