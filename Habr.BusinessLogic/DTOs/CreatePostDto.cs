namespace Habr.BusinessLogic.DTOs
{
    public class CreatePostDto
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPublished { get; set; }
    }
}