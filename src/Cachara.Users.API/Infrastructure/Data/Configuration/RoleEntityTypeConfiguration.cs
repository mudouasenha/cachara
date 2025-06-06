using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(t => t.Id)
            .IsUnique();

        builder.Property(t => t.Id)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired();

        builder.HasMany(p => p.UserRoles)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("timezone('UTC', now())");
        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.Deleted)
            .HasDefaultValue(false)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Version)
            .HasDefaultValueSql("gen_random_bytes(8)")
            .IsRowVersion();
    }
}
