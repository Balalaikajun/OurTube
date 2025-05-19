namespace OurTube.Application.DTOs.Comment;

public class PagedCommentDto
{
    public IEnumerable<CommentGetDto> Comments { get; set; }
    public int? NextAfter { get; set; }
    public bool HasMore { get; set; }
}