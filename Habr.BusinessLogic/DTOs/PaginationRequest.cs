using System.ComponentModel;
using Habr.Common;

namespace Habr.BusinessLogic.DTOs
{
    public class PaginationRequest
    {
        [DefaultValue(Constants.Pagination.DefaultPageNumber)]
        public int PageNumber { get; set; }

        [DefaultValue(Constants.Pagination.DefaultPageSize)]
        public int PageSize { get; set; }

    }
}
