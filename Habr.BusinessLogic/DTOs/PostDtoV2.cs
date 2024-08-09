namespace Habr.BusinessLogic.DTOs
{
    public class PostDtoV2
    {
        public string Title { get; set; }
        public string PublishedAt { get; set; }
        public AuthorDto Author { get; set; }
    }
}
