using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IRatingService
    {
        Task RatePostAsync(RatePostDto ratePostDto);
        Task CalculateAverageRatingsAsync();
    }
}
