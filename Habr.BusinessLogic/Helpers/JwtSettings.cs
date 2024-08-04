namespace Habr.BusinessLogic.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int TokenLifetimeMinutes { get; set; }
        public int RefreshTokenLifetimeDays { get; set; }
    }
}
