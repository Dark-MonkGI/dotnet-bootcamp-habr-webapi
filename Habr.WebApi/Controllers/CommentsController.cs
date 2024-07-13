using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Habr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("{postId}")]
        public async Task<IActionResult> AddCommentAsync(int postId, [FromBody] AddCommentRequest addCommentRequest)
        {
            var comment = await _commentService.AddComment(new AddCommentDto
            {
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                PostId = postId,
                Text = addCommentRequest.Text
            });

            return StatusCode(201, comment);
        }

        [HttpPost("{parentCommentId}/reply")]
        public async Task<IActionResult> AddReplyAsync(int parentCommentId, [FromBody] AddReplyRequest addReplyRequest)
        {
            var comment = await _commentService.AddReply(new AddReplyDto
            {
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                ParentCommentId = parentCommentId,
                Text = addReplyRequest.Text
            });

            return StatusCode(201, comment);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _commentService.DeleteComment(commentId, userId);
            return Ok();
        }
    }
}
