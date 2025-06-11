using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}