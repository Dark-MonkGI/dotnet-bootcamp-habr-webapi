using Habr.WebApi.Interfaces;
using Habr.WebApi.Exceptions;
using Habr.WebApi.Middleware;

namespace Habr.WebApi.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddSingleton<IExceptionMapper, ExceptionToProblemDetailsMapper>();
            services.AddSingleton<IExceptionHandler, DefaultGlobalExceptionHandler>();
            return services;
        }

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
            return app;
        }
    }
}