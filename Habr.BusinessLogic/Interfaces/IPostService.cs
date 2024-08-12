using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using X.PagedList;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDtoV1>> GetAllPublishedPosts();
        Task<IPagedList<PostDtoV2>> GetAllPublishedPostsV2(int pageNumber, int pageSize);
        Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId);
        Task<IEnumerable<Post>> GetAllUserPosts(int userId);
        Task<Post> CreatePost(CreatePostDto createPostDto);
        Task<Post> GetPostWithCommentsAsync(int postId, int userId);
        Task UpdatePost(UpdatePostDto updatePostDto);
        Task DeletePost(int postId, int userId);
        Task PublishPostAsync(int postId, int userId);
        Task MovePostToDraftAsync(int postId, int userId);
        Task<PostDetailsDto> GetPostDetailsAsync(int postId);
        Task<Post> GetPostByIdAsync(int postId, int userId);
        Task UpdatePostAsAdmin(UpdatePostDto updatePostDto);
        Task DeletePostAsAdmin(int postId);
    }
}
