using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities;

public class Video
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private set; }

    [MaxLength(150)] [Required] public string Title { get; set; }

    [MaxLength(5000)] [Required] public string Description { get; set; } = string.Empty;

    [Required] public int LikesCount { get; private set; } = 0;

    [Required] public int DislikeCount { get; private set; } = 0;

    [Required] public int CommentsCount { get; set; } = 0;

    [Required] public int ViewsCount { get; set; } = 0;

    [Required] public DateTime Created { get; private set; } = DateTime.UtcNow;

    public string ApplicationUserId { get; private set; }

    //Navigation
    [Required]
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser User { get; private set; }

    [Required] public VideoPreview Preview { get; private set; }

    [Required] public VideoSource Source { get; private set; }

    [Required] public TimeSpan Duration { get; private set; }

    public ICollection<VideoPlaylist> Files { get; private set; }
    public ICollection<VideoVote> Votes { get; private set; }
    public ICollection<Comment> Comments { get; private set; }
    public ICollection<VideoView> Views { get; private set; }
    public ICollection<VideoTags> Tags { get; private set; }

    public Video()
    {
    }

    public Video(
        string title,
        string description,
        VideoPreview preview,
        VideoSource source,
        string applicationUserId,
        ICollection<VideoPlaylist> files,
        ICollection<VideoTags> tags,
        TimeSpan duration)
    {
        Title = title;
        Description = description;
        Preview = preview;
        Source = source;
        ApplicationUserId = applicationUserId;
        Files = files;
        Tags = tags;
        Duration = duration;
    }

    public void UpdateLikesCount(int delta) =>
        LikesCount = Math.Max(0, LikesCount + delta);


    public void UpdateDislikesCount(int delta) =>
        DislikeCount = Math.Max(0, DislikeCount + delta);
}