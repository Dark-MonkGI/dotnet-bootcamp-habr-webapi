using Habr.BusinessLogic.Resources;

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

            if (title.Length > 200)
            {
                throw new ArgumentException(Messages.PostTitleTooLong);
            }
        }

        public static void ValidateText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(Messages.PostTextRequired);
            }

            if (text.Length > 2000)
            {
                throw new ArgumentException(Messages.PostTextTooLong);
            }
        }
    }
}
