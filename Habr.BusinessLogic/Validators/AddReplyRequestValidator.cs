using Habr.BusinessLogic.DTOs;
using Habr.Common;

namespace Habr.BusinessLogic.Validators
{
    public class AddReplyRequestValidator : BaseTextValidator<AddReplyRequest>
    {
        public AddReplyRequestValidator()
        {
            ValidateTextField(x => x.Text, Constants.Comment.TextMaxLength);
        }
    }
}
