using Habr.BusinessLogic.Resources;
using Habr.Common;

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

            if (text.Length > Constants.Comment.TextMaxLength)
            {
                throw new ArgumentException(string.Format(Messages.CommentTextTooLong, Constants.Comment.TextMaxLength));
            }
        }
    }
}
