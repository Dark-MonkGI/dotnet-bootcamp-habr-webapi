namespace Habr.WebApi.Interfaces
{
    public interface IExceptionHandler
    {
        Task<bool> TryHandleAsync(HttpContext context, Exception exception);
    }
}
