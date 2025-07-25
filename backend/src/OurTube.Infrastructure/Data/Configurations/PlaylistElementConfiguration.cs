using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class PlaylistElementConfiguration : IEntityTypeConfiguration<PlaylistElement>
{
    public void Configure(EntityTypeBuilder<PlaylistElement> builder)
    {
        builder.HasOne(pe => pe.Playlist)
            .WithMany(p => p.PlaylistElements)
            .HasForeignKey(pe => pe.PlaylistId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pe => pe.Video)
            .WithMany()
            .HasForeignKey(pe => pe.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pe => new { pe.PlaylistId, pe.VideoId })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
    }
}