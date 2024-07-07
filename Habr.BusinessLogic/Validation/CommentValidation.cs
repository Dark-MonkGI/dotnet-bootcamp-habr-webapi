using Habr.BusinessLogic.Resources;

namespace Habr.BusinessLogic.Validation
{
    public static class CommentValidation
    {
        public static void ValidateCommentText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(Messages.CommentTextEmpty);
            }

            if (text.Length > 200) 
            {
                throw new ArgumentException(Messages.CommentTextTooLong);
            }
        }
    }
}
