namespace Habr.BusinessLogic.Helpers
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int TokenLifetimeDays { get; set; }
    }
}
