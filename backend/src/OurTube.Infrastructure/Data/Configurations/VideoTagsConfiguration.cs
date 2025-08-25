using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoTagsConfiguration : IEntityTypeConfiguration<VideoTags>
{
    public void Configure(EntityTypeBuilder<VideoTags> builder)
    {
        builder.Property(vt => vt.VideoId)
            .IsRequired();

        builder.Property(vt => vt.TagId)
            .IsRequired();

        builder.HasOne(vt => vt.Video)
            .WithMany(v => v.Tags)
            .HasForeignKey(vt => vt.VideoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vt => vt.Tag)
            .WithMany()
            .HasForeignKey(vt => vt.TagId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(vt => new { vt.VideoId, vt.TagId })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
        
        builder.HasQueryFilter(ua => !ua.IsDeleted);
    }
}