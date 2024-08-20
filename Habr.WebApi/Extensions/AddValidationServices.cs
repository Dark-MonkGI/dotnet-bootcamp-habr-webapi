using FluentValidation;
using Habr.BusinessLogic.Validators;
using Habr.WebApi.Filters;

namespace Habr.WebApi.Extensions
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<RatePostRequestValidator>();
            services.AddScoped(typeof(ValidationFilter<>));

            return services;
        }
    }
}
