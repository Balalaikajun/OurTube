using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.DTOs.Comment
{
    public class CommentGetDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int? ParentId { get; set; }
        public bool Edited { get; set; } = false;
        public bool? Vote { get; set; }
        public int LikesCount { get; set; }
        public int DisLikesCount { get; set; }
        //Navigation
        public ApplicationUserDto User { get; set; }
        public ICollection<CommentGetDto> Childs { get; set; }
    }
}
