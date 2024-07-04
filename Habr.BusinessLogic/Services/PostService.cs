﻿using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;

namespace Habr.BusinessLogic.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostDto>> GetAllPublishedPosts()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.IsPublished && !p.IsDeleted)
                .OrderByDescending(p => p.PublishedDate)
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    AuthorEmail = p.User.Email,
                    PublicationDate = p.PublishedDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DraftPostDto>> GetUserDraftPosts(int userId)
        {
            return await _context.Posts
                .Where(p => 
                    !p.IsPublished && 
                    p.UserId == userId && 
                    !p.IsDeleted)
                .OrderByDescending(p => p.Updated)
                .Select(p => new DraftPostDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    CreatedAt = p.Created,
                    UpdatedAt = p.Updated
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllUserPosts(int userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.Created)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Post> CreatePost(
            int userId, 
            string title, 
            string text, 
            bool isPublished)
        {

            var post = new Post
            {
                UserId = userId,
                Title = title,
                Text = text,
                IsPublished = isPublished,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                PublishedDate = isPublished ? DateTime.UtcNow : (DateTime?)null
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> GetPostWithCommentsAsync(int postId, int userId)
        {
            var post =  await _context.Posts
                .Include(p => p.Comments)
                .Where(p => 
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            return post;
        }

        public async Task<Post> GetPostByIdAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            return post;
        }

        public async Task UpdatePost(int postId, int userId, UpdatePostDto updatePostDto)
        {
            var existingPost = await GetPostByIdAsync(postId, userId);

            if (existingPost == null)
            {
                throw new ArgumentException("Post not found.");
            }

            if (existingPost.IsPublished)
            {
                throw new InvalidOperationException("A published post cannot be edited. Move it to drafts first.");
            }

            existingPost.Title = updatePostDto.Title;
            existingPost.Text = updatePostDto.Text;
            existingPost.Updated = DateTime.UtcNow;

            _context.Posts.Update(existingPost);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePost(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("The post does not exist.");
            }

            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task PublishPostAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("\nThe post does not exist.");
            }

            if (post.IsPublished)
            {
                throw new InvalidOperationException("\nThe post is already published.");
            }

            post.IsPublished = true;
            post.PublishedDate = DateTime.UtcNow;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task MovePostToDraftAsync(int postId, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Comments)
                .Where(p =>
                    p.Id == postId &&
                    p.UserId == userId &&
                    !p.IsDeleted)
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("\nThe post does not exist.");
            }

            if (!post.IsPublished)
            {
                throw new InvalidOperationException("\nThe post is already in drafts.");
            }

            if (post.Comments.Any())
            {
                throw new InvalidOperationException("\nThe post cannot be moved to drafts because it has comments.");
            }

            post.IsPublished = false;
            post.PublishedDate = null;
            post.Updated = DateTime.UtcNow;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task<PostDetailsDto> GetPostDetailsAsync(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Where(p => p.Id == postId && p.IsPublished && !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException("Post not found or is not published.");
            }

            var postDetails = new PostDetailsDto
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                AuthorEmail = post.User.Email,
                PublicationDate = post.PublishedDate,
                Comments = post.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    Created = c.Created,
                    UserName = c.User.Name,
                    ParentCommentId = c.ParentCommentId
                }).ToList()
            };

            return postDetails;
        }
    }
}
