namespace Habr.WebApi.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int TokenLifetimeDays { get; set; }
    }
}
