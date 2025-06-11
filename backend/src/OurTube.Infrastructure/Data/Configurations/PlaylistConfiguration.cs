using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(p => p.Count)
            .IsRequired();

        builder.Property(p => p.IsSystem)
            .IsRequired();

        builder.Property(p => p.ApplicationUserId)
            .IsRequired();

        builder.HasOne(p => p.ApplicationUser)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}