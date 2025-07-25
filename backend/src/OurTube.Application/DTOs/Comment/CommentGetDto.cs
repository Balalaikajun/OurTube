using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.DTOs.Comment;

public class CommentGetDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public DateTime? Deleted { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public bool? Vote { get; set; }
    public uint LikesCount { get; set; }
    public uint ChildsCount { get; set; }
    public uint DislikesCount { get; set; }

    //Navigation
    public ApplicationUserDto User { get; set; }
}