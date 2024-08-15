﻿using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Resources;
using Habr.DataAccess;
using Habr.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Habr.BusinessLogic.Services
{
    public class RatingService : IRatingService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public RatingService(DataContext context, IMapper mapper, ILogger<PostService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task RatePostAsync(RatePostDto ratePostDto)
        {
            var post = await _context.Posts
                .Where(p => 
                    p.Id == ratePostDto.PostId && 
                    !p.IsDeleted && 
                    p.IsPublished)
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (post == null)
            {
                throw new ArgumentException(Messages.PostNotFoundOrUnpublished);
            }

            var rating = await _context.Ratings
                .SingleOrDefaultAsync(r => 
                    r.PostId == ratePostDto.PostId && 
                    r.UserId == ratePostDto.UserId
                );

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

            await _context.SaveChangesAsync();
        }

        public async Task CalculateAverageRatingsAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.Ratings)
                .Where(p => p.Ratings.Any())
                .ToListAsync();

            foreach (var post in posts)
            {
                var averageRating = post.Ratings.Average(r => r.Value);
                post.AverageRating = Math.Round(averageRating, 2);

                _context.Posts.Update(post);
            }

            await _context.SaveChangesAsync();
        }
    }
}
