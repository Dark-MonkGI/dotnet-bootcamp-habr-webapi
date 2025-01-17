﻿using Habr.BusinessLogic.DTOs;
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
            return await _commentService.GetCommentsByPost(postId, CancellationToken.None);
        }

        public async Task<Comment> AddCommentAsync(AddCommentDto addCommentDto)
        {
            return await _commentService.AddComment(addCommentDto, CancellationToken.None);
        }

        public async Task<Comment> AddReplyAsync(AddReplyDto addReplyDto)
        {
            return await _commentService.AddReply(addReplyDto, CancellationToken.None);
        }

        public async Task<IEnumerable<Comment>> GetUserCommentsAsync(int userId)
        {
            return await _commentService.GetUserComments(userId, CancellationToken.None);
        }

        public async Task DeleteCommentAsync(int commentId, int userId)
        {
            await _commentService.DeleteComment(commentId, userId, CancellationToken.None);
        }
    }
}
