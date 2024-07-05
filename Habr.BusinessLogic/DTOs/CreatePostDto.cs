namespace Habr.WebApi.DTOs
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPublished { get; set; }
    }
}