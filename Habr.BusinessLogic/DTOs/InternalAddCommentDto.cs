namespace Habr.BusinessLogic.DTOs
{
    public class InternalAddCommentDto
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Text { get; set; }
    }
}
