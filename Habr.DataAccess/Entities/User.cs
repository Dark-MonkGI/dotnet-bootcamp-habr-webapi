using Microsoft.AspNetCore.Identity;

namespace Habr.DataAccess.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Rating> Ratings { get; set; }

        public User()
        {
            Posts = new HashSet<Post>();
            Comments = new HashSet<Comment>();
            Ratings = new HashSet<Rating>();
        }
    }
}
