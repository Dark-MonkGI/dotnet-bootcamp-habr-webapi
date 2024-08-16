using FluentValidation;
using System.Linq.Expressions;
using Habr.BusinessLogic.Resources;

namespace Habr.BusinessLogic.Validators
{
    public abstract class BaseTextValidator<T> : AbstractValidator<T> where T : class
    {
        protected void ValidateTextField(Expression<Func<T, string>> selector, int maxLength)
        {
            var memberExpression = selector.Body as MemberExpression;
            string fieldName = memberExpression?.Member.Name ?? ValidationMessages.DefaultFieldName;

            RuleFor(selector)
                .NotEmpty().WithMessage(string.Format(ValidationMessages.ValidationRequired, fieldName))
                .MaximumLength(maxLength).WithMessage(string.Format(ValidationMessages.ValidationMaxLength, fieldName, maxLength));
        }
    }
}
