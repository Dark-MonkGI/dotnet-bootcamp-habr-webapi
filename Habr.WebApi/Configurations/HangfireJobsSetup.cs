using Habr.BusinessLogic.Interfaces;
using Hangfire;

namespace Habr.WebApi.Configurations
{
    public static class HangfireJobsSetup
    {
        public static void ConfigureRecurringJobs(IConfiguration configuration)
        {
            var dailyRatingCalcCron = configuration["Hangfire:Jobs:DailyRatingCalcCron"];

            RecurringJob.AddOrUpdate<IRatingService>(
                "daily-rating-calc",
                service => service.CalculateAverageRatingsAsync(CancellationToken.None),
                dailyRatingCalcCron);
        }
    }
}
