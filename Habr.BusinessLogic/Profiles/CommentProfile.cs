using AutoMapper;
using Habr.BusinessLogic.DTOs;
using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
              .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
              .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name))
              .ForMember(dest => dest.ParentCommentId, opt => opt.MapFrom(src => src.ParentCommentId));

            CreateMap<InternalAddReplyDto, Comment>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ParentCommentId, opt => opt.MapFrom(src => src.ParentCommentId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.PostId, opt => opt.Ignore())
                .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore());

            CreateMap<AddCommentDto, Comment>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ParentComment, opt => opt.Ignore())
                .ForMember(dest => dest.Post, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore());
        }
    }
}
