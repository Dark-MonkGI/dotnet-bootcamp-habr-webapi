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
            var post = await context.Posts
                .Where(p => p.Id == postId && p.IsPublished)
                .FirstOrDefaultAsync();

            if (post == null)
            {
                throw new InvalidOperationException("You cannot comment on unpublished posts!");
            }

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
            var parentComment = await context.Comments
                .Where(c => c.Id == parentCommentId && c.Post.IsPublished)
                .Include(c => c.Post)
                .FirstOrDefaultAsync();

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
            var comment = await context.Comments
                .Include(c => c.Replies)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
            {
                return false;
            }

            await DeleteReplies(comment);

            context.Comments.Remove(comment);

            await context.SaveChangesAsync();
            return true;
        }

        private async Task DeleteReplies(Comment comment)
        {
            foreach (var reply in comment.Replies.ToList())
            {
                await context.Entry(reply).Collection(r => r.Replies).LoadAsync();

                await DeleteReplies(reply);

                context.Comments.Remove(reply);
            }
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
