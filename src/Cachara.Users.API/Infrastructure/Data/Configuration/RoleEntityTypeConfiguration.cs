using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class RoleEntityTypeConfiguration : BaseEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(t => t.Name)
            .IsUnique();

        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired();

        builder.HasMany(p => p.UserRoles)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.AssignedRole)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
