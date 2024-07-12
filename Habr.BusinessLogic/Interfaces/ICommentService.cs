using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> AddComment(AddCommentDto addCommentDto);
        Task<Comment> AddReply(InternalAddReplyDto addReplyDto);
        Task DeleteComment(int commentId, int userId);
        Task<IEnumerable<Comment>> GetCommentsByPost(int postId);
        Task<IEnumerable<Comment>> GetUserComments(int userId);
    }
}