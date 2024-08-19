using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Habr.BusinessLogic.Services
{
    public class RatingService : IRatingService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RatingService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task RatePostAsync(RatePostDto ratePostDto, CancellationToken cancellationToken)
        {
            var post = await _context.Posts
                .Include(p => p.Ratings)
                .Where(p =>
                    p.Id == ratePostDto.PostId &&
                    !p.IsDeleted &&
                    p.IsPublished)
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                throw new ArgumentException(Messages.PostNotFoundOrUnpublished);
            }

            var rating = post.Ratings.SingleOrDefault(r => r.UserId == ratePostDto.UserId);

            if (rating == null)
            {
                rating = _mapper.Map<Rating>(ratePostDto);
                rating.CreatedAt = DateTime.UtcNow;
                _context.Ratings.Add(rating);
            }
            else
            {
                _mapper.Map(ratePostDto, rating);
                rating.CreatedAt = DateTime.UtcNow;
                _context.Ratings.Update(rating);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CalculateAverageRatingsAsync(CancellationToken cancellationToken)
        {
            var posts = await _context.Posts
                .Include(p => p.Ratings)
                .Where(p => p.Ratings.Any())
                .ToListAsync(cancellationToken);

            foreach (var post in posts)
            {
                var averageRating = post.Ratings.Average(r => r.Value);
                post.AverageRating = Math.Round(averageRating, 2);

                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
