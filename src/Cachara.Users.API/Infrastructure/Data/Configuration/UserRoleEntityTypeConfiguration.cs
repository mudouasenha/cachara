using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class UserRoleEntityTypeConfiguration : BaseEntityTypeConfiguration<UserRole>
{
    public new void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.Property(t => t.UserId)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(t => t.RoleId)
            .HasConversion<string>()
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(t => t.AssignedDate)
            .IsRequired();

        builder.HasOne(p => p.Role)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(p => p.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
