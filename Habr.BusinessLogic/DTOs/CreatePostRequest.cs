﻿namespace Habr.BusinessLogic.DTOs
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public bool IsPublished { get; set; }
    }
}