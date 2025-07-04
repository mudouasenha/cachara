using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class UserInterestEntityTypeConfiguration : BaseEntityTypeConfiguration<UserInterest>
{
    public new void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.Property(t => t.UserProfileId)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(t => t.Interest)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(p => p.UserProfile).WithMany(p => p.Interests)
            .HasForeignKey(p => p.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
