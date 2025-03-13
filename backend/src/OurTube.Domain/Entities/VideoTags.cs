using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OurTube.Domain.Entities;

[PrimaryKey(nameof(VideoId), nameof(TagId))]
public class VideoTags
{
    public int TagId { get; set; }
    public int VideoId { get; set; }
    
    public Tag Tag { get; set; }
    public Video Video { get; set; }
}