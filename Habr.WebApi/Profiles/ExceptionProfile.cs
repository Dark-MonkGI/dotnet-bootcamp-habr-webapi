using AutoMapper;
using Habr.WebApi.Exceptions;
using Habr.WebApi.Resources;

namespace Habr.WebApi.Profiles
{
    public class ExceptionProfile : Profile
    {
        public ExceptionProfile()
        {
            CreateMap<ValidationException, ProblemDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusCodes.Status400BadRequest))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => Messages.ValidationErrorMessage))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Extensions, opt => opt.MapFrom(src => new Dictionary<string, object> { { "errors", src.Errors } }));

            CreateMap<ArgumentException, ProblemDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusCodes.Status400BadRequest))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => Messages.BadRequestTitle))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Message));

            CreateMap<InvalidOperationException, ProblemDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusCodes.Status400BadRequest))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => Messages.BadRequestTitle))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Message));

            CreateMap<UnauthorizedAccessException, ProblemDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusCodes.Status401Unauthorized))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => Messages.UnauthorizedTitle))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Message));

            CreateMap<Exception, ProblemDetails>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusCodes.Status500InternalServerError))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => Messages.InternalServerErrorTitle))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Message));
        }
    }
}