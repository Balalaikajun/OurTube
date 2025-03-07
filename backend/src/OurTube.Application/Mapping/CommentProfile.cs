using AutoMapper;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Mapping
{
    public class CommentProfile:Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentGetDTO>();
        }
    }
}
