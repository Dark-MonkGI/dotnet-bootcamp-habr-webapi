using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using X.PagedList;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IPostService
    {
        Task<IEnumerable<PostDtoV1>> GetAllPublishedPosts(CancellationToken cancellationToken = default);
        Task<IPagedList<PostDtoV2>> GetAllPublishedPostsV2(PaginatedParametersDto paginatedParametersDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Post>> GetAllUserPosts(int userId, CancellationToken cancellationToken = default);
        Task<Post> CreatePost(CreatePostDto createPostDto, CancellationToken cancellationToken = default);
        Task<Post> GetPostWithCommentsAsync(int postId, int userId, CancellationToken cancellationToken = default);
        Task UpdatePost(UpdatePostDto updatePostDto, CancellationToken cancellationToken = default);
        Task DeletePost(int postId, int userId, CancellationToken cancellationToken = default);
        Task PublishPostAsync(int postId, int userId, CancellationToken cancellationToken = default);
        Task MovePostToDraftAsync(int postId, int userId, CancellationToken cancellationToken = default);
        Task<PostDetailsDto> GetPostDetailsAsync(int postId, CancellationToken cancellationToken = default);
        Task<Post> GetPostByIdAsync(int postId, int userId, CancellationToken cancellationToken = default);
        Task UpdatePostAsAdmin(UpdatePostDto updatePostDto, CancellationToken cancellationToken = default);
        Task DeletePostAsAdmin(int postId, CancellationToken cancellationToken = default);
    }
}
