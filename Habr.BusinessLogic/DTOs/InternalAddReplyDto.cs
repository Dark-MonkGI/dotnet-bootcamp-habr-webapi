namespace Habr.BusinessLogic.DTOs
{
    public class InternalAddReplyDto
    {
        public int UserId { get; set; }
        public int ParentCommentId { get; set; }
        public string Text { get; set; }
    }
}
