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

        public async Task<Comment> AddComment(AddCommentDto addCommentDto, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Where(p => 
                    p.Id == addCommentDto.PostId && 
                    p.IsPublished && 
                    !p.IsDeleted)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                throw new InvalidOperationException(Messages.CannotCommentUnpublished);
            }

            var comment = _mapper.Map<Comment>(addCommentDto);
            comment.Created = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return comment;
        }

        public async Task<Comment> AddReply(AddReplyDto addReplyDto, CancellationToken cancellationToken)
        {
            var parentComment = await _context.Comments
                .Where(c => 
                    c.Id == addReplyDto.ParentCommentId 
                    && c.Post.IsPublished && 
                    !c.Post.IsDeleted)
                .Include(c => c.Post)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (parentComment == null)
            {
                throw new ArgumentException(Messages.CommentNotFound);
            }

            var comment = _mapper.Map<Comment>(addReplyDto);
            comment.PostId = parentComment.PostId;
            comment.Created = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return comment;
        }

        public async Task DeleteComment(int commentId, int userId, CancellationToken cancellationToken)
        {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .SingleOrDefaultAsync(c => 
                    c.Id == commentId && 
                    c.UserId == userId, 
                    cancellationToken);

            if (comment == null)
            {
                throw new ArgumentException(Messages.DeleteCommentPermissionDenied);
            }

            await DeleteReplies(comment, cancellationToken);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task DeleteReplies(Comment comment, CancellationToken cancellationToken)
        {
            foreach (var reply in comment.Replies.ToList())
            {
                await _context.Entry(reply).Collection(r => r.Replies).LoadAsync(cancellationToken);
                await DeleteReplies(reply, cancellationToken);
                _context.Comments.Remove(reply);
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPost(int postId, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Comment>> GetUserComments(int userId, CancellationToken cancellationToken)
        {
            return await _context.Comments
                .Where(c => c.UserId == userId)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteCommentAsAdmin(int commentId, CancellationToken cancellationToken)
        {
            var comment = await _context.Comments
                .Include(c => c.Replies)
                .SingleOrDefaultAsync(c => c.Id == commentId, cancellationToken);

            if (comment == null)
            {
                throw new ArgumentException(Messages.CommentNotFound);
            }

            await DeleteReplies(comment, cancellationToken);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
