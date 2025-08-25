namespace OurTube.Domain.Entities;

public class Comment : Base
{
    public Guid VideoId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public string Text { get; set; }
    public Guid? ParentId { get; set; }
    public int LikesCount { get; set; } = 0;
    public int ChildsCount { get; set; } = 0;
    public int DislikesCount { get; set; } = 0;
    
    //Navigation
    public ApplicationUser User { get; set; }
    public Comment? Parent { get; set; }
    public ICollection<Comment> Childs { get; set; }
    public ICollection<CommentVote> Votes { get; set; }
}