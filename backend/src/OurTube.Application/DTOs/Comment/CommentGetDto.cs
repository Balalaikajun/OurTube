using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.DTOs.Comment;

public class CommentGetDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
    public int? ParentId { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public bool? Vote { get; set; }
    public int LikesCount { get; set; }
    public int ChildsCount { get; set; }
    public int DislikesCount { get; set; }

    //Navigation
    public ApplicationUserDto User { get; set; }
}