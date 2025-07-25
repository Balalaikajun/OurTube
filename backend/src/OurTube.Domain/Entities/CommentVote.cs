namespace OurTube.Domain.Entities;

public class CommentVote : Base
{
    public Guid CommentId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public bool Type { get; set; }
    
    //Navigation
    public Comment Comment { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}