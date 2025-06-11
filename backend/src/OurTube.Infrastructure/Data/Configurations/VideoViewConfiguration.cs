using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoViewConfiguration : IEntityTypeConfiguration<VideoView>
{
    public void Configure(EntityTypeBuilder<VideoView> builder)
    {
        builder.HasKey(vv => new { vv.VideoId, vv.ApplicationUserId });

        builder.Property(vv => vv.VideoId).IsRequired();
        builder.Property(vv => vv.ApplicationUserId).IsRequired();

        builder.Property(vv => vv.EndTime).IsRequired();
        builder.Property(vv => vv.DateTime).IsRequired();

        builder.HasOne(vv => vv.Video)
            .WithMany(v => v.Views)
            .HasForeignKey(vv => vv.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vv => vv.ApplicationUser)
            .WithMany()
            .HasForeignKey(vv => vv.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}