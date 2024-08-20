using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Validators
{
    public class RegisterUserRequestValidator : BaseUserValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
            : base(x => x.Email, x => x.Password)
        {
        }
    }
}
