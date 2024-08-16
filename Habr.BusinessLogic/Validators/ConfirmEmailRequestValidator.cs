using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Validators
{
    public class ConfirmEmailRequestValidator : BaseUserValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailRequestValidator()
            : base(x => x.Email, x => x.Password)
        {
        }
    }
}
