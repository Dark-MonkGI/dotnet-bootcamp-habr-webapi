using Habr.BusinessLogic.Interfaces;
using Habr.BusinessLogic.Services;

namespace Habr.WebApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<ITokenService, TokenService>();

            return services;
        }
    }
}
