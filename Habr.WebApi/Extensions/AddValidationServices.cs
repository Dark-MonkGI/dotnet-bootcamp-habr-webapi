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
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<ConfirmEmailRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AuthenticateUserRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<CreatePostRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdatePostRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AddCommentRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AddReplyRequestValidator>();

            services.AddScoped(typeof(ValidationFilter<>));

            return services;
        }
    }
}
