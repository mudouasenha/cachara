using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class UserFollowerEntityTypeConfiguration : BaseEntityTypeConfiguration<UserFollower>
{
    public void Configure(EntityTypeBuilder<UserFollower> builder)
    {
        builder.Property(t => t.UserId)
            .HasMaxLength(36)
            .IsRequired();
        
        builder.Property(t => t.FollowerId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(t => t.FollowedAt)
            .IsRequired();
    }
}