using System.ComponentModel.DataAnnotations;
using static Habr.Common.Constants;

namespace Habr.DataAccess.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }

        [Range(RatingConstants.MinValue, RatingConstants.MaxValue)]
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }

        public Post Post { get; set; }
        public User User { get; set; }
    }
}
