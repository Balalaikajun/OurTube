using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class CommentVoteConfiguration : IEntityTypeConfiguration<CommentVote>
{
    public void Configure(EntityTypeBuilder<CommentVote> builder)
    {
        builder.HasKey(cv => new { cv.CommentId, cv.ApplicationUserId });

        builder.Property(cv => cv.Type)
            .IsRequired();

        builder.Property(cv => cv.Created)
            .IsRequired();

        builder.HasOne(cv => cv.Comment)
            .WithMany(c => c.Votes)
            .HasForeignKey(cv => cv.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cv => cv.ApplicationUser)
            .WithMany()
            .HasForeignKey(cv => cv.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}