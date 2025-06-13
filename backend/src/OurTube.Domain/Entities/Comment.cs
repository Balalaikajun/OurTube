namespace OurTube.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int VideoId { get; set; }
    public string ApplicationUserId { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Updated { get; set; } = null;
    public DateTime? Deleted { get; set; } = null;
    public int? ParentId { get; set; }
    public bool IsEdited { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public int LikesCount { get; set; } = 0;
    public int ChildsCount { get; set; } = 0;
    public int DislikesCount { get; set; } = 0;

    //Navigation
    public ApplicationUser User { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Childs { get; set; }
    public ICollection<CommentVote> Votes { get; set; }
}