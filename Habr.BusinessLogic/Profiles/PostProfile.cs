using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;
using System.Globalization;

namespace Habr.BusinessLogic.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PaginationRequest, PaginatedParametersDto>()
                .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));

            CreateMap<Post, PostDtoV1>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedDate.HasValue ? src.PublishedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null));

            CreateMap<Post, PostDtoV2>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedDate.HasValue ? src.PublishedDate.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => new AuthorDto
                {
                    Email = src.User.Email,
                    Name = src.User.UserName
                }));

            CreateMap<Post, PostDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PublicationDate, opt => opt.MapFrom(src => src.PublishedDate))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<CreatePostDto, Post>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => src.IsPublished))
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<UpdatePostDto, Post>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PostId));


            CreateMap<Post, DraftPostDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Created))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.Updated));
        }
    }
}
