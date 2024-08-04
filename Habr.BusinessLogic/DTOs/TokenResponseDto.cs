namespace Habr.BusinessLogic.DTOs
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
    }
}
