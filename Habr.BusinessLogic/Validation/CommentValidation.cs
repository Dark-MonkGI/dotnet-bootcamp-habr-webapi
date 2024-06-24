using System;

namespace Habr.BusinessLogic.Validation
{
    public static class CommentValidation
    {
        public static void ValidateCommentText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Comment text cannot be empty.");
            }

            if (text.Length > 200) 
            {
                throw new ArgumentException("Comment text cannot exceed 200 characters.");
            }
        }
    }
}
