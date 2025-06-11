using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class PlaylistElementConfiguration : IEntityTypeConfiguration<PlaylistElement>
{
    public void Configure(EntityTypeBuilder<PlaylistElement> builder)
    {
        builder.HasKey(pe => new { pe.PlaylistId, pe.VideoId });

        builder.Property(pe => pe.AddedAt)
            .IsRequired();

        builder.HasOne(pe => pe.Playlist)
            .WithMany(p => p.PlaylistElements)
            .HasForeignKey(pe => pe.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pe => pe.Video)
            .WithMany()
            .HasForeignKey(pe => pe.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}