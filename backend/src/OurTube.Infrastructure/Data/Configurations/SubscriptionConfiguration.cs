using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => new { s.SubscriberId, s.SubscribedToId }); // Установка составного ключа

        builder.Property(s => s.Created)
            .IsRequired();

        builder.HasOne(s => s.Subscriber)
            .WithMany(u => u.SubscribedTo)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.SubscribedTo)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.SubscribedToId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}