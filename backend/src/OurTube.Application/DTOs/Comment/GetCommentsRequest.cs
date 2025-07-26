namespace OurTube.Application.DTOs.Comment;

public record GetCommentsRequest(
    Guid VideoId, 
    int Limit,
    int After,
    Guid SessionId,
    Guid? UserId,
    Guid? ParentId = null,
    bool Reload = false);