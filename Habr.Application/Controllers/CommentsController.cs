using Habr.BusinessLogic.DTOs;
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

        public async Task<Comment> AddCommentAsync(InternalAddCommentDto addCommentDto)
        {
            return await _commentService.AddComment(addCommentDto);
        }

        public async Task<Comment> AddReplyAsync(InternalAddReplyDto addReplyDto)
        {
            return await _commentService.AddReply(addReplyDto);
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
