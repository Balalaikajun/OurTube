using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OurTube.Domain.Entities;

[PrimaryKey(nameof(VideoId), nameof(TagId))]
public class VideoTags
{
    public int TagId { get;private set; }
    public int VideoId { get; private set; }
    
    public Tag Tag { get; private set; }
    public Video Video { get; private set; }

    public VideoTags()
    {
        
    }

    public VideoTags( int tagId)
    {
        TagId = tagId;
    }
    
    public VideoTags(int videoId, int tagId)
    {
        VideoId = videoId;
        TagId = tagId;
    }
}