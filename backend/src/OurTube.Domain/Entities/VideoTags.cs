namespace OurTube.Domain.Entities;

public class VideoTags
{
    public VideoTags()
    {
    }

    public VideoTags(int tagId)
    {
        TagId = tagId;
    }

    public VideoTags(int videoId, int tagId)
    {
        VideoId = videoId;
        TagId = tagId;
    }

    public int TagId { get; private set; }
    public int VideoId { get; private set; }

    public Tag Tag { get; }
    public Video Video { get; }
}