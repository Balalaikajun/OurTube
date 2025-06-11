using OurTube.Domain.Events.VideoVote;

namespace OurTube.Domain.Entities;

public class VideoVote : BaseEntity
{
    public VideoVote()
    {
    }

    public VideoVote(int videoId, string applicationUserId, bool type)
    {
        VideoId = videoId;
        ApplicationUserId = applicationUserId;
        Type = type;

        CreateEvent();
    }

    public int VideoId { get; }
    public string ApplicationUserId { get; }
    public bool Type { get; private set; }
    public DateTime Created { get; private set; } = DateTime.UtcNow;

    //Navigation
    public Video Video { get; }
    public ApplicationUser ApplicationUser { get; }

    public void Update(bool type)
    {
        if (Type == type)
            return;

        var oldType = Type;
        Type = type;
        Created = DateTime.UtcNow;

        UpdateEvent(oldType);
    }

    private void CreateEvent()
    {
        AddDomainEvent(new VideoVoteCreateEvent(
            VideoId,
            ApplicationUserId,
            Type,
            Created));
    }

    private void UpdateEvent(bool oldValue)
    {
        AddDomainEvent(new VideoVoteUpdateEvent(
            VideoId,
            ApplicationUserId,
            oldValue,
            Type,
            Created));
    }

    public void RemoveEvent()
    {
        AddDomainEvent(new VideoVoteDeleteEvent(
            VideoId,
            ApplicationUserId,
            Type));
    }
}