using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Users.API.Infrastructure.Data.Configuration;

    public class UserEntityTypeConfiguration : BaseEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.FullName)
                .HasMaxLength(64)
                .IsRequired();
            
            builder.Property(t => t.UserName)
                .HasMaxLength(64)
                .IsRequired();
            
            builder.Property(t => t.Email)
                .HasMaxLength(320)
                .IsRequired();
            
            builder.Property(t => t.Password)
                .HasMaxLength(100)
                .IsRequired();
            
            builder.Property(t => t.DateOfBirth)
                .IsRequired();
            
            builder.Property(t => t.Subscription)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(t => t.ProfilePictureUrl)
                .HasMaxLength(2048)
                .IsRequired();

            builder.OwnsOne(p => p.Settings)
                .WithOwner(p => p.User)
                .HasForeignKey(p => p.UserId);

            // builder.HasMany(p => p.Followers)
            //     .WithOne(p => p.User)
            //     .HasForeignKey(p => p.UserId);
            
            // builder
            //     .HasMany(p => p.Following)
            //     .WithOne(p => p.Follower)
            //     .HasForeignKey(p => p.FollowerId);

            builder
                .HasMany(p => p.UserRoles)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

        }
    }