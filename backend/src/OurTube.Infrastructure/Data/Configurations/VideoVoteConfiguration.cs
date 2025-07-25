using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoVoteConfiguration : IEntityTypeConfiguration<VideoVote>
{
    public void Configure(EntityTypeBuilder<VideoVote> builder)
    {
        builder.Property(vv => vv.VideoId)
            .IsRequired();

        builder.Property(vv => vv.ApplicationUserId)
            .IsRequired();

        builder.Property(vv => vv.Type)
            .IsRequired();

        builder.HasOne(vv => vv.Video)
            .WithMany(v => v.Votes)
            .HasForeignKey(vv => vv.VideoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vv => vv.ApplicationUser)
            .WithMany()
            .HasForeignKey(vv => vv.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(vv => new { vv.VideoId, vv.ApplicationUserId })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
    }
}