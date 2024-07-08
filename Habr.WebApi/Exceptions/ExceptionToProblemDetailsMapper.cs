using AutoMapper;
using Habr.WebApi.Interfaces;

namespace Habr.WebApi.Exceptions
{
    public class ExceptionToProblemDetailsMapper : IExceptionMapper
    {
        private readonly IMapper _mapper;

        public ExceptionToProblemDetailsMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ProblemDetails Map(Exception exception)
        {
            return _mapper.Map<ProblemDetails>(exception);
        }
    }
}
