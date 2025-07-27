using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasOne(s => s.Subscriber)
            .WithMany(u => u.SubscribedTo)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SubscribedTo)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.SubscribedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.SubscribedToId, s.SubscriberId })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();
        
        builder.HasQueryFilter(ua => !ua.IsDeleted);
    }
}