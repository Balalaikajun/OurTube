using OurTube.Domain.Events.VideoVote;

namespace OurTube.Domain.Entities;

public class VideoVote : Base
{
    public VideoVote()
    {
    }

    public VideoVote(Guid videoId, Guid applicationUserId, bool type)
    {
        VideoId = videoId;
        ApplicationUserId = applicationUserId;
        Type = type;

        CreateEvent();
    }

    public Guid VideoId { get; }
    public Guid ApplicationUserId { get; }
    public bool Type { get; private set; }
    
    //Navigation
    public Video Video { get; }
    public ApplicationUser ApplicationUser { get; }

    public void Update(bool type)
    {
        if (Type == type)
            return;

        var oldType = Type;
        Type = type;

        UpdateEvent(oldType);
    }

    private void CreateEvent()
    {
        AddDomainEvent(new VideoVoteCreateEvent(
            VideoId,
            ApplicationUserId,
            Type));
    }

    private void UpdateEvent(bool oldValue)
    {
        AddDomainEvent(new VideoVoteUpdateEvent(
            VideoId,
            ApplicationUserId,
            oldValue,
            Type));
    }

    public void RemoveEvent()
    {
        AddDomainEvent(new VideoVoteDeleteEvent(
            VideoId,
            ApplicationUserId,
            Type));
    }
}