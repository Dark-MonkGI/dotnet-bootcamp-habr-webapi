using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.DataAccess.Entities;
using Habr.WebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var posts = await _postService.GetAllPublishedPosts();

                if (posts == null || !posts.Any())
                {
                    return NotFound("No posts found.");
                }

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("drafts")]
        public async Task<IActionResult> GetUserDraftPostsAsync()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var posts = await _postService.GetUserDraftPosts(userId);

                if (posts == null || !posts.Any())
                {
                    return NotFound("No draft posts found.");
                }

                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostDto createPostDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var post = await _postService.CreatePost(
                    userId, 
                    createPostDto.Title, 
                    createPostDto.Text, 
                    createPostDto.IsPublished);

                return StatusCode(201, post);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePostAsync(int postId, [FromBody] UpdatePostDto updatePostDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var postToUpdate = new Post
                {
                    Id = postId,
                    Title = updatePostDto.Title,
                    Text = updatePostDto.Text,
                    Updated = DateTime.UtcNow,
                    UserId = userId
                };

                await _postService.UpdatePost(postToUpdate);

                return Ok("Post updated!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePostAsync(int postId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _postService.DeletePost(postId, userId);
                return Ok("Post deleted!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("publish/{postId}")]
        public async Task<IActionResult> PublishPostAsync(int postId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _postService.PublishPostAsync(postId, userId);
                return Ok("Post published!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("move-to-draft/{postId}")]
        public async Task<IActionResult> MovePostToDraftAsync(int postId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _postService.MovePostToDraftAsync(postId, userId);
                return Ok("Post moved to drafts successfully!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{postId}/details")]
        public async Task<IActionResult> GetPostDetailsAsync(int postId)
        {
            try
            {
                var postDetails = await _postService.GetPostDetailsAsync(postId);
                return Ok(postDetails);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
