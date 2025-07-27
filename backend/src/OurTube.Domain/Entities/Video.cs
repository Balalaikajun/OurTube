using NpgsqlTypes;

namespace OurTube.Domain.Entities;

public class Video : Base
{
    public Video()
    {
    }

    public Video(
        string title,
        string description,
        VideoPreview preview,
        VideoSource source,
        Guid applicationUserId,
        ICollection<VideoPlaylist> files,
        ICollection<VideoTags> tags,
        TimeSpan duration)
    {
        Title = title;
        Description = description;
        ApplicationUserId = applicationUserId;
        Duration = duration;
        Preview = preview;
        Source = source;
        Files = files;
        Tags = tags;
    }

    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public int LikesCount { get; private set; }
    public int DislikesCount { get; private set; }
    public int CommentsCount { get; set; } = 0;
    public int ViewsCount { get; set; } = 0;
    public Guid ApplicationUserId { get; private set; }
    public ApplicationUser User { get; }
    public VideoPreview Preview { get; private set; }
    public VideoSource Source { get; private set; }
    public TimeSpan Duration { get; private set; }
    public NpgsqlTsVector SearchVector { get; private set; }

    public ICollection<VideoPlaylist> Files { get; private set; }
    public ICollection<VideoVote> Votes { get; }
    public ICollection<Comment> Comments { get; }
    public ICollection<VideoView> Views { get; }
    public ICollection<VideoTags> Tags { get; private set; }

    public void UpdateLikesCount(int delta)
    {
        LikesCount = Math.Max(0, LikesCount + delta);
    }

    public void UpdateDislikesCount(int delta)
    {
        DislikesCount = Math.Max(0, DislikesCount + delta);
    }
}