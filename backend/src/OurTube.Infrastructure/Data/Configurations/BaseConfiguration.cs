using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data.Configurations;

public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Base
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedDate)
            .IsRequired();

        builder.Property(e => e.UpdatedDate)
            .IsRequired();

        builder.Property(e => e.DeletedDate);

        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.HasIndex(v => v.IsDeleted);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}