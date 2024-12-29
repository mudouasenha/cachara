using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

public class UserSettingsEntityTypeConfiguration : BaseEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.Property(t => t.UserId)
            .HasMaxLength(36)
            .IsRequired();
            
        builder.Property(t => t.IsPrivate)
            .IsRequired();
        
        builder.Property(t => t.ReceiveNotifications)
            .IsRequired();
        
        builder.Property(t => t.ShowEmail)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithOne(p => p.Settings)
            .HasForeignKey<UserSettings>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}