using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> AddComment(int userId, int postId, string text);
        Task<Comment> AddReply(int userId, int parentCommentId, string text);
        Task DeleteComment(int commentId, int userId);
        Task<IEnumerable<Comment>> GetCommentsByPost(int postId);
        Task<IEnumerable<Comment>> GetUserComments(int userId);
    }
}