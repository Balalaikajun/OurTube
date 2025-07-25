using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoPlaylistConfiguration : IEntityTypeConfiguration<VideoPlaylist>
{
    public void Configure(EntityTypeBuilder<VideoPlaylist> builder)
    {
        builder.Property(vp => vp.VideoId)
            .IsRequired();

        builder.Property(vp => vp.Resolution)
            .IsRequired();

        builder.Property(vp => vp.FileName)
            .IsRequired()
            .HasMaxLength(125);

        builder.Property(vp => vp.Bucket)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasOne(vp => vp.Video)
            .WithMany(v => v.Files)
            .HasForeignKey(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(vp => new { vp.VideoId, vp.Resolution })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
    }
}