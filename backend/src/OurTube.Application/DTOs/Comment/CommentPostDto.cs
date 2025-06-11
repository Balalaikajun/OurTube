namespace OurTube.Application.DTOs.Comment;

public class CommentPostDto
{
    public int VideoId { get; set; }
    public string Text { get; set; }
    public int? ParentId { get; set; }
}