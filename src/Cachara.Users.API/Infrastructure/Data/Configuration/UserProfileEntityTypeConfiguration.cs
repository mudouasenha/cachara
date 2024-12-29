using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class UserProfileEntityTypeConfiguration : BaseEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.Property(t => t.UserId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(t => t.Bio)
            .IsRequired();
            
        builder.Property(t => t.WebsiteUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasOne(p => p.User);
    }
}