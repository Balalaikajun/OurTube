using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Text)
            .HasMaxLength(1500)
            .IsRequired();

        builder.Property(c => c.Created)
            .IsRequired();

        builder.Property(c => c.IsEdited)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired();

        builder.Property(c => c.LikesCount)
            .IsRequired();

        builder.Property(c => c.DislikesCount)
            .IsRequired();

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne<Video>()
            .WithMany(v => v.Comments)
            .HasForeignKey(c => c.VideoId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Childs)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}