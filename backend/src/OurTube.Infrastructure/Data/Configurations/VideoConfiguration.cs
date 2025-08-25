using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;
using NpgsqlTypes;

namespace OurTube.Infrastructure.Data.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.Property(v => v.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(v => v.Description)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(v => v.LikesCount)
            .IsRequired();

        builder.Property(v => v.DislikesCount)
            .IsRequired();

        builder.Property(v => v.CommentsCount)
            .IsRequired();

        builder.Property(v => v.ViewsCount)
            .IsRequired();

        builder.Property(v => v.Duration)
            .HasColumnType("interval")
            .IsRequired();

        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasGeneratedTsVectorColumn(
            v => v.SearchVector,
            "simple",
            v => new { v.Title, v.Description });
        builder.HasIndex(v => v.SearchVector).HasMethod("GIN");

        builder.HasQueryFilter(ua => !ua.IsDeleted);
    }
}