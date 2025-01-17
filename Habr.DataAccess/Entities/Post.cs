﻿namespace Habr.DataAccess.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool IsDeleted { get; set; }
        public double? AverageRating { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }


        public ICollection<Comment> Comments { get; set; }
        public ICollection<Rating> Ratings { get; set; }

        public Post()
        {
            Comments = new HashSet<Comment>();
            Ratings = new HashSet<Rating>();
        }
    }
}
