using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;

namespace Habr.BusinessLogic.Services
{
    public class CommentService
    {
        private readonly DataContext _context;

        public CommentService(DataContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddComment(int userId, int postId, string text)
        {
            var post = await _context.Posts
                .Where(p => p.Id == postId && p.IsPublished)
                .AsNoTracking()
                .SingleOrDefaultAsync();

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

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> AddReply(int userId, int parentCommentId, string text)
        {
            var parentComment = await _context.Comments
                .Where(c => c.Id == parentCommentId && c.Post.IsPublished)
                .Include(c => c.Post)
                .AsNoTracking()
                .SingleOrDefaultAsync();

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

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task DeleteComment(int commentId, int userId)
        {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .SingleOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

            if (comment == null)
            {
                throw new ArgumentException("\nComment not found or you do not have permission to delete it.");
            }

            await DeleteReplies(comment);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }

        private async Task DeleteReplies(Comment comment)
        {
            foreach (var reply in comment.Replies.ToList())
            {
                await _context.Entry(reply).Collection(r => r.Replies).LoadAsync();
                await DeleteReplies(reply);
                _context.Comments.Remove(reply);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPost(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetUserComments(int userId)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
