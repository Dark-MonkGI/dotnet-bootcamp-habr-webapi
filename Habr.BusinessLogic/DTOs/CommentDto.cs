namespace Habr.BusinessLogic.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public int? ParentCommentId { get; set; }
    }
}