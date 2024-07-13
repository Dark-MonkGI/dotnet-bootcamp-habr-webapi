namespace Habr.WebApi.Exceptions
{
    public class ProblemDetails
    {
        public int? Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();
    }
}