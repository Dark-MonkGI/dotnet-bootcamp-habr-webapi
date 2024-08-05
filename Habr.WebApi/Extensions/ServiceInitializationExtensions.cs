using Habr.DataAccess;
using Habr.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Habr.WebApi.Extensions
{
    public static class ServiceInitializationExtensions
    {
        public static async Task InitializeDatabaseAndRolesAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();

                await RoleInitializer.AdminInitializeAsync(services);
            }
        }
    }
}
