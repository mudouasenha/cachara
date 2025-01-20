using Cachara.Shared.Domain.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Shared.Infrastructure.Data.EF.Configuration;

public class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class, IEntity<string>, ISoftDeletable, IVersable, IModifiable
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(t => t.Id).IsClustered(false);
        builder.Property(p => p.Id).HasMaxLength(36);

        builder.Property(p => p.CreatedAt);
        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.Deleted)
            .HasDefaultValue(false)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Version).IsRowVersion();
    }
}
