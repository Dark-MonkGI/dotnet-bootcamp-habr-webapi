using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Habr.WebApi.Resources;

namespace Habr.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            var posts = await _postService.GetAllPublishedPosts();

            if (posts == null || !posts.Any())
            {
                return NotFound(Messages.NoPostsFound);
            }

            return Ok(posts);
        }

        [HttpGet("drafts")]
        public async Task<IActionResult> GetUserDraftPostsAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var posts = await _postService.GetUserDraftPosts(userId);

            if (posts == null || !posts.Any())
            {
                return NotFound(Messages.NoDraftPosts);
            }

            return Ok(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto createPostDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var post = await _postService.CreatePost(createPostDto, userId);

            return StatusCode(201, post);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePostAsync([FromRoute] int postId, [FromBody] UpdatePostDto updatePostDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _postService.UpdatePost(postId, userId, updatePostDto);
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _postService.DeletePost(postId, userId);
            return Ok();
        }

        [HttpPost("{postId}/publish")]
        public async Task<IActionResult> PublishPostAsync([FromRoute] int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _postService.PublishPostAsync(postId, userId);
            return Ok();
        }

        [HttpPost("{postId}/move-to-draft")]
        public async Task<IActionResult> MovePostToDraftAsync([FromRoute] int postId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _postService.MovePostToDraftAsync(postId, userId);
            return Ok();
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostDetailsAsync([FromRoute] int postId)
        {
            var postDetails = await _postService.GetPostDetailsAsync(postId);
            return Ok(postDetails);
        }
    }
}
