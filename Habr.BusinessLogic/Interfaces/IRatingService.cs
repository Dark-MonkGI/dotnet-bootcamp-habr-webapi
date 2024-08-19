using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IRatingService
    {
        Task RatePostAsync(RatePostDto ratePostDto, CancellationToken cancellationToken = default);
        Task CalculateAverageRatingsAsync(CancellationToken cancellationToken = default);
    }
}
