using Habr.BusinessLogic.DTOs;

namespace Habr.BusinessLogic.Validators
{
    public class AuthenticateUserRequestValidator : BaseUserValidator<AuthenticateUserRequest>
    {
        public AuthenticateUserRequestValidator()
            : base(x => x.Email, x => x.Password)
        {
        }
    }
}
