using FluentValidation;
using Habr.BusinessLogic.Resources;
using Habr.Common;
using System.Linq.Expressions;

namespace Habr.BusinessLogic.Validators
{
    public class BaseUserValidator<T> : AbstractValidator<T> where T : class
    {
        public BaseUserValidator(Expression<Func<T, string>> emailExpression, Expression<Func<T, string>> passwordExpression)
        {
            RuleFor(emailExpression)
                .NotEmpty().WithMessage(ValidationMessages.EmailRequired)
                .MaximumLength(Constants.User.EmailMaxLength).WithMessage(string.Format(ValidationMessages.EmailMaxLength, Constants.User.EmailMaxLength))
                .Matches(Constants.User.EmailPattern).WithMessage(ValidationMessages.EmailInvalidFormat);

            RuleFor(passwordExpression)
                .NotEmpty().WithMessage(ValidationMessages.PasswordRequired)
                .MinimumLength(Constants.User.PasswordMinLength).WithMessage(ValidationMessages.PasswordTooShort);
        }
    }
}

