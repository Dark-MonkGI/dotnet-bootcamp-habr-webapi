using System;

namespace Habr.BusinessLogic.Validation
{
    public static class PostValidation
    {
        public static void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required.");
            }

            if (title.Length > 200)
            {
                throw new ArgumentException("Title must be less than 200 symbols.");
            }
        }

        public static void ValidateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text is required.");
            }

            if (text.Length > 2000)
            {
                throw new ArgumentException("Text must be less than 2000 symbols.");
            }
        }
    }
}
