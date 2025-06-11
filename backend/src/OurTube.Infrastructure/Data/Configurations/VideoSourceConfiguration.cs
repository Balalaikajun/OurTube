using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoSourceConfiguration : IEntityTypeConfiguration<VideoSource>
{
    public void Configure(EntityTypeBuilder<VideoSource> builder)
    {
        builder.HasKey(vs => vs.VideoId);

        builder.Property(vs => vs.FileName)
            .IsRequired()
            .HasMaxLength(125);

        builder.Property(vs => vs.Bucket)
            .IsRequired()
            .HasMaxLength(25);

        builder.HasOne(vs => vs.Video)
            .WithOne(v => v.Source)
            .HasForeignKey<VideoSource>(vs => vs.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}