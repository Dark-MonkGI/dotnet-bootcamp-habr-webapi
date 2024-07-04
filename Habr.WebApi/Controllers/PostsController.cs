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
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    return BadRequest(ex.Message);
                }

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
                    UserId = userId
                };

                await _postService.UpdatePost(postToUpdate);

                return Ok("Post updated!");
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    return BadRequest(ex.Message);
                }

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
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    return BadRequest(ex.Message);
                }

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{postId}/publish")]
        public async Task<IActionResult> PublishPostAsync(int postId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _postService.PublishPostAsync(postId, userId);
                return Ok("Post published!");
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    return BadRequest(ex.Message);
                }

                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{postId}/move-to-draft")]
        public async Task<IActionResult> MovePostToDraftAsync(int postId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _postService.MovePostToDraftAsync(postId, userId);
                return Ok("Post moved to drafts successfully!");
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is InvalidOperationException)
                {
                    return BadRequest(ex.Message);
                }

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPostDetailsAsync(int postId)
        {
            try
            {
                var postDetails = await _postService.GetPostDetailsAsync(postId);
                return Ok(postDetails);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    return NotFound(ex.Message);
                }

                return StatusCode(500, ex.Message);
            }
        }
    }
}
