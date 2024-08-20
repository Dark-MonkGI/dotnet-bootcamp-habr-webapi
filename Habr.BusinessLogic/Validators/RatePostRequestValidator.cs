using FluentValidation;
using Habr.BusinessLogic.DTOs;
using Habr.BusinessLogic.Resources;
using Habr.Common;

namespace Habr.BusinessLogic.Validators
{
    public class RatePostRequestValidator : AbstractValidator<RatePostRequest>
    {
        public RatePostRequestValidator()
        {
            RuleFor(x => x.RatingValue)
                .InclusiveBetween(Constants.RatingConstants.MinValue, Constants.RatingConstants.MaxValue)
                .WithMessage(string.Format(ValidationMessages.RatingValueRange, Constants.RatingConstants.MinValue, Constants.RatingConstants.MaxValue));
        }
    }
}
