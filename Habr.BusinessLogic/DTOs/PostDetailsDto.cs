namespace Habr.BusinessLogic.DTOs
{
    public class PostDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}