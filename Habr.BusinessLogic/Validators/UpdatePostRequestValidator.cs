using Habr.BusinessLogic.DTOs;
using Habr.Common;

namespace Habr.BusinessLogic.Validators
{
    public class UpdatePostRequestValidator : BaseTextValidator<UpdatePostRequest>
    {
        public UpdatePostRequestValidator()
        {
            ValidateTextField(x => x.Title, Constants.Post.TitleMaxLength);
            ValidateTextField(x => x.Text, Constants.Post.TextMaxLength);
        }
    }
}
