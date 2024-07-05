using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDto>> GetAllPublishedPosts();
        Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId);
        Task<IEnumerable<Post>> GetAllUserPosts(int userId);
        Task<Post> CreatePost(int userId, string title, string text, bool isPublished);
        Task<Post> GetPostWithCommentsAsync(int postId, int userId);
        Task UpdatePost(int postId, int userId, UpdatePostDto updatePostDto);
        Task DeletePost(int postId, int userId);
        Task PublishPostAsync(int postId, int userId);
        Task MovePostToDraftAsync(int postId, int userId);
        Task<PostDetailsDto> GetPostDetailsAsync(int postId);
        Task<Post> GetPostByIdAsync(int postId, int userId);
    }
}
