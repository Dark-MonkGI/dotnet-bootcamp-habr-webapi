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
        public async Task<IActionResult> AddCommentAsync(int postId, [FromBody] AddCommentDto addCommentDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var comment = await _commentService.AddComment(userId, postId, addCommentDto.Text);

                return StatusCode(201, comment);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentException) 
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{parentCommentId}/reply")]
        public async Task<IActionResult> AddReplyAsync(int parentCommentId, [FromBody] AddReplyDto addReplyDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var comment = await _commentService.AddReply(userId, parentCommentId, addReplyDto.Text);

                return StatusCode(201, comment);
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentAsync(int commentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _commentService.DeleteComment(commentId, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException || ex is ArgumentException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.Message);
            }
        }
    }
}
