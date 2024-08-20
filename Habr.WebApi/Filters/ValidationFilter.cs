using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Habr.WebApi.Resources;

namespace Habr.WebApi.Filters
{
    public sealed class ValidationFilter<T> : IEndpointFilter
    {
        private readonly IValidator<T> _validator;

        public ValidationFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.GetArgument<T>(0);

            var validationResult = await _validator.ValidateAsync(argument);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary())
                {
                    Detail = Messages.ValidationFailed
                };
                return Results.BadRequest(problemDetails);
            }

            return await next(context);
        }
    }
}
