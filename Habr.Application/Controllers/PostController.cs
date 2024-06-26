﻿using Habr.BusinessLogic.Services;
using Habr.DataAccess.Entities;

namespace Habr.Application.Controllers
{
    public class PostController
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _postService.GetAllPosts();
        }

        public async Task<IEnumerable<Post>> GetUserDraftPostsAsync(int userId)
        {
            return await _postService.GetUserDraftPosts(userId);
        }

        public async Task<IEnumerable<Post>> GetUserPostsAsync(int userId)
        {
            return await _postService.GetAllUserPosts(userId);
        }

        public async Task<Post> CreatePostAsync(int userId, string title, string text, bool isPublished)
        {
            return await _postService.CreatePost(userId, title, text, isPublished);
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId)
        {
            return await _postService.GetPostWithCommentsAsync(postId, userId);
        }

        public async Task UpdatePostAsync(Post post)
        {
            await _postService.UpdatePost(post);
        }
        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            return await _postService.DeletePost(postId, userId);
        }
    }
}