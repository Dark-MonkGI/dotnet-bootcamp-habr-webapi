using Microsoft.EntityFrameworkCore;
using Habr.DataAccess.Entities;
using Habr.DataAccess;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Resources;
using AutoMapper;

namespace Habr.BusinessLogic.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CommentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Comment> AddComment(AddCommentDto addCommentDto)
        {
            var post = await _context.Posts
                .Where(p => p.Id == addCommentDto.PostId && p.IsPublished)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new InvalidOperationException(Messages.CannotCommentUnpublished);
            }

            var comment = _mapper.Map<Comment>(addCommentDto);
            comment.Created = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<Comment> AddReply(InternalAddReplyDto addReplyDto)
        {
            var parentComment = await _context.Comments
                .Where(c => c.Id == addReplyDto.ParentCommentId && c.Post.IsPublished)
                .Include(c => c.Post)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (parentComment == null)
            {
                throw new ArgumentException(Messages.CommentNotFound);
            }

            var comment = _mapper.Map<Comment>(addReplyDto);
            comment.PostId = parentComment.PostId;
            comment.Created = DateTime.UtcNow;

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
                throw new ArgumentException(Messages.DeleteCommentPermissionDenied);
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
