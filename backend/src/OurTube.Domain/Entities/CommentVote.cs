namespace OurTube.Domain.Entities;

public class CommentVote
{
    public int CommentId { get; set; }
    public string ApplicationUserId { get; set; }
    public bool Type { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;

    //Navigation
    public Comment Comment { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}