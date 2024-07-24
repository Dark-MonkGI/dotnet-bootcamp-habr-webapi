using AutoMapper;
using Habr.BusinessLogic.DTOs;

namespace Habr.WebApi
{
    public class WebApiMappingProfile : Profile
    {
        public WebApiMappingProfile()
        {
            CreateMap<CreatePostRequest, CreatePostDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.IsPublished))
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<UpdatePostRequest, UpdatePostDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PostId, opt => opt.Ignore());

            CreateMap<RegisterUserRequest, RegisterUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed));

            CreateMap<AuthenticateUserRequest, AuthenticateUserDto>()
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<ConfirmEmailRequest, AuthenticateUserDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed));
        }
    }
}