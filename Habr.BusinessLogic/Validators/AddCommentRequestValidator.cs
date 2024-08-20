using Habr.BusinessLogic.DTOs;
using Habr.Common;

namespace Habr.BusinessLogic.Validators
{
    public class AddCommentRequestValidator : BaseTextValidator<AddCommentRequest>
    {
        public AddCommentRequestValidator()
        {
            ValidateTextField(x => x.Text, Constants.Comment.TextMaxLength);
        }
    }
}
