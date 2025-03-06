using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.DTOs.Comment
{
    public class CommentGetDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; } 
        public DateTime Updated { get; set; }
        public int? ParentId { get; set; }
        public bool Edited { get; set; } = false;
        public int LikesCount { get; set; } 
        public int DisLikesCount { get; set; } 
        //Navigation
        public ApplicationUserDTO User { get; set; }
        public ICollection<CommentGetDTO> Childs { get; set; }
    }
}
