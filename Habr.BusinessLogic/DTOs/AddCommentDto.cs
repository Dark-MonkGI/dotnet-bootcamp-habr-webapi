namespace Habr.BusinessLogic.DTOs
{
    public class AddCommentDto
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
    }
}
