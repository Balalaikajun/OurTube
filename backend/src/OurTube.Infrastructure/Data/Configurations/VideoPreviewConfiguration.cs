using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoPreviewConfiguration : IEntityTypeConfiguration<VideoPreview>
{
    public void Configure(EntityTypeBuilder<VideoPreview> builder)
    {
        builder.HasKey(vp => vp.VideoId);

        builder.Property(vp => vp.FileName)
            .IsRequired()
            .HasMaxLength(125);

        builder.Property(vp => vp.Bucket)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasOne(vp => vp.Video)
            .WithOne(v => v.Preview)
            .HasForeignKey<VideoPreview>(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}