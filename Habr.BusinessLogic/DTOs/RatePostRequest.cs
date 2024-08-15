using static Habr.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Habr.BusinessLogic.DTOs
{
    public class RatePostRequest
    {
        public int PostId { get; set; }

        [Range(RatingConstants.MinValue, RatingConstants.MaxValue)]
        public int RatingValue { get; set; }
    }
}
