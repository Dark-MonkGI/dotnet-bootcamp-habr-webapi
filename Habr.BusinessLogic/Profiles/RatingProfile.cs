using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<RatePostDto, Rating>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.RatingValue))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<RatePostRequest, RatePostDto>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.RatingValue, opt => opt.MapFrom(src => src.RatingValue))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
