using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class UserAvatarConfiguration : IEntityTypeConfiguration<UserAvatar>
{
    public void Configure(EntityTypeBuilder<UserAvatar> builder)
    {
        builder.HasKey(ua => ua.UserId);

        builder.Property(ua => ua.FileName)
            .HasMaxLength(125)
            .IsRequired();

        builder.Property(ua => ua.Bucket)
            .HasMaxLength(25)
            .IsRequired();

        builder.HasOne(ua => ua.ApplicationUser)
            .WithOne(ua => ua.UserAvatar)
            .HasForeignKey<UserAvatar>(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}