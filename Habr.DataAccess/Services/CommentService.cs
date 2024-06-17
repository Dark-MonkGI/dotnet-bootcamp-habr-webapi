using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Services
{
    public class CommentService
    {
        private readonly DataContext context;

        public CommentService(DataContext context)
        {
            this.context = context;
        }

        public async Task<Comment> AddComment(int userId, int postId, string text)
        {
            var comment = new Comment
            {
                UserId = userId,
                PostId = postId,
                Text = text,
                Created = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> AddReply(int userId, int parentCommentId, string text)
        {
            var parentComment = await context.Comments.FindAsync(parentCommentId);
            if (parentComment == null)
            {
                throw new ArgumentException("Parent comment not found.");
            }

            var comment = new Comment
            {
                UserId = userId,
                ParentCommentId = parentCommentId,
                PostId = parentComment.PostId,
                Text = text,
                Created = DateTime.UtcNow
            };

            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return comment;
        }

        public async Task<bool> DeleteComment(int commentId, int userId)
        {
            var comment = await context.Comments.FindAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                return false;
            }

            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPost(int postId)
        {
            return await context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetUserComments(int userId)
        {
            return await context.Comments
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }
}
