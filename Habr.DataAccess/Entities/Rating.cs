namespace Habr.DataAccess.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}
