using Habr.BusinessLogic.DTOs;
using Habr.Common;

namespace Habr.BusinessLogic.Validators
{
    public class CreatePostRequestValidator : BaseTextValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            ValidateTextField(x => x.Title, Constants.Post.TitleMaxLength);
            ValidateTextField(x => x.Text, Constants.Post.TextMaxLength);
        }
    }
}
