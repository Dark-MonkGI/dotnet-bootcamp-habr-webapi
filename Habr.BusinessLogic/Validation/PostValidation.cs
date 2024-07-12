using Habr.BusinessLogic.Resources;
using Habr.Common;

namespace Habr.BusinessLogic.Validation
{
    public static class PostValidation
    {
        public static void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(Messages.PostTitleRequired);
            }

            if (title.Length > Constants.Post.TitleMaxLength)
            {
                throw new ArgumentException(string.Format(Messages.PostTitleTooLong, Constants.Post.TitleMaxLength));
            }
        }

        public static void ValidateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(Messages.PostTextRequired);
            }

            if (text.Length > Constants.Post.TextMaxLength)
            {
                throw new ArgumentException(string.Format(Messages.PostTextTooLong, Constants.Post.TextMaxLength));
            }
        }
    }
}
