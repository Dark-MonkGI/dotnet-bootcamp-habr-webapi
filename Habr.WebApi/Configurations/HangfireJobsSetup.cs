using Habr.BusinessLogic.Interfaces;
using Hangfire;

namespace Habr.WebApi.Configurations
{
    public static class HangfireJobsSetup
    {
        public static void ConfigureRecurringJobs()
        {
            RecurringJob.AddOrUpdate<IRatingService>(
                "daily-rating-calc",
                service => service.CalculateAverageRatingsAsync(),
                Cron.Daily);
        }
    }
}
