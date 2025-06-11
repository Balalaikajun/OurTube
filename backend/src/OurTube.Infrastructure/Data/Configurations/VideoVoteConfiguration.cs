using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoVoteConfiguration : IEntityTypeConfiguration<VideoVote>
{
    public void Configure(EntityTypeBuilder<VideoVote> builder)
    {
        builder.HasKey(vv => new { vv.VideoId, vv.ApplicationUserId });

        builder.Property(vv => vv.VideoId)
            .IsRequired();

        builder.Property(vv => vv.ApplicationUserId)
            .IsRequired();

        builder.Property(vv => vv.Type)
            .IsRequired();

        builder.Property(vv => vv.Created)
            .IsRequired();

        builder.HasOne(vv => vv.Video)
            .WithMany(v => v.Votes)
            .HasForeignKey(vv => vv.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vv => vv.ApplicationUser)
            .WithMany()
            .HasForeignKey(vv => vv.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}