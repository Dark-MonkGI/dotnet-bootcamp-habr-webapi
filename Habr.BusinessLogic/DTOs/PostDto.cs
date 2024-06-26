namespace Habr.BusinessLogic.DTOs
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorEmail { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}