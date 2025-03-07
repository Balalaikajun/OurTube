using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Comment
{
    public class CommentPostDTO
    {
        public int VideoId { get; set; }
        public string Text { get; set; }
        public int? ParentId { get; set; }
    }
}
