using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.SubscribersCount)
            .IsRequired();

        builder.Property(u => u.SubscribedToCount)
            .IsRequired();

        builder.Property(u => u.Created)
            .IsRequired();

        builder.HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}