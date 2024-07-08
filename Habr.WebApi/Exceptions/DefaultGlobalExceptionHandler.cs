using Habr.WebApi.Interfaces;
using System.Text.Json;

namespace Habr.WebApi.Exceptions
{
    public class DefaultGlobalExceptionHandler : IExceptionHandler
    {
        private readonly IExceptionMapper _exceptionMapper;

        public DefaultGlobalExceptionHandler(IExceptionMapper exceptionMapper)
        {
            _exceptionMapper = exceptionMapper;
        }

        public async Task<bool> TryHandleAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/problem+json";
            var problemDetails = _exceptionMapper.Map(exception);
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
            return true;
        }
    }
}