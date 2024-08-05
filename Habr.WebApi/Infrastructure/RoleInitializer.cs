using Microsoft.AspNetCore.Identity;
using Habr.DataAccess.Entities;
using Habr.Common;
using Habr.WebApi.Resources;

namespace Habr.WebApi.Infrastructure
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            if (!int.TryParse(configuration["Jwt:RefreshTokenLifetimeDays"], out var refreshTokenLifetimeDays))
            {
                throw new InvalidOperationException(Messages.AlreadyAuthenticated);
            }

            var roleNames = new string[] { Constants.Roles.Admin, Constants.Roles.User };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }

            var adminEmail = configuration["AdminUser:Email"];
            var adminPassword = configuration["AdminUser:Password"];
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Name = adminEmail.Split('@')[0],
                    UserName = adminEmail.Split('@')[0],
                    Email = adminEmail,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    RefreshToken = Guid.NewGuid().ToString(),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenLifetimeDays)
                };

                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, Constants.Roles.Admin);
            }
        }
    }
}
