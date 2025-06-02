using AutoMapper;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentGetDto>()
                .AfterMap((comment, dto) =>
                {
                    if (comment.IsDeleted)
                        dto.Text = "";
                });
        }
    }
}
