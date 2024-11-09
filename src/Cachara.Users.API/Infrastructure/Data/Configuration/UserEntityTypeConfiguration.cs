using Cachara.Domain.Entities;
using Cachara.Users.API.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Data.Persistence.Configuration;

    public class UserEntityTypeConfiguration : BaseEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.Email)
                .HasMaxLength(320)
                .IsRequired();
            
            builder.Property(t => t.FullName)
                .IsRequired();

            builder.Property(t => t.UserName)
                .HasMaxLength(64)
                .IsRequired();
            
            builder.Property(t => t.Password)
                .HasMaxLength(100)
                .IsRequired();
        }
    }