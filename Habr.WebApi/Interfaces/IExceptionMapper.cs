using Habr.WebApi.Exceptions;

namespace Habr.WebApi.Interfaces
{
    public interface IExceptionMapper
    {
        ProblemDetails Map(Exception exception);
    }
}
