using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> AddComment(AddCommentDto addCommentDto, CancellationToken cancellationToken = default);
        Task<Comment> AddReply(AddReplyDto addReplyDto, CancellationToken cancellationToken = default);
        Task DeleteComment(int commentId, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetCommentsByPost(int postId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Comment>> GetUserComments(int userId, CancellationToken cancellationToken = default);
        Task DeleteCommentAsAdmin(int commentId, CancellationToken cancellationToken = default);
    }
}