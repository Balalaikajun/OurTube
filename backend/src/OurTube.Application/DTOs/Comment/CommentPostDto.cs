namespace OurTube.Application.DTOs.Comment;

public class CommentPostDto
{
    public Guid VideoId { get; set; }
    public string Text { get; set; }
    public Guid? ParentId { get; set; } = null;
}