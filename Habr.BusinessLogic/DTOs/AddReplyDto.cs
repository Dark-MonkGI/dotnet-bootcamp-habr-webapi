namespace Habr.BusinessLogic.DTOs
{
    public class AddReplyDto
    {
        public int ParentCommentId { get; set; }
        public string Text { get; set; }
    }
}