﻿namespace Habr.BusinessLogic.DTOs
{
    public class UpdatePostDto
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}