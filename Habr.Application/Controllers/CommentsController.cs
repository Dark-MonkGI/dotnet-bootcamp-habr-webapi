using Habr.BusinessLogic.Services;
using Habr.DataAccess.Entities;

namespace Habr.Application.Controllers
{
    public class CommentsController
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostAsync(int postId)
        {
            return await _commentService.GetCommentsByPost(postId);
        }

        public async Task<Comment> AddCommentAsync(int userId, int postId, string text)
        {
            return await _commentService.AddComment(userId, postId, text);
        }

        public async Task<Comment> AddReplyAsync(int userId, int parentCommentId, string text)
        {
            return await _commentService.AddReply(userId, parentCommentId, text);
        }

        public async Task<IEnumerable<Comment>> GetUserCommentsAsync(int userId)
        {
            return await _commentService.GetUserComments(userId);
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            await _commentService.DeleteComment(commentId, userId);
        }
    }
}
