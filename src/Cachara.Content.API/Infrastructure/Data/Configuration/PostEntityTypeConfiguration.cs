using Cachara.Content.API.Domain.Entities;
using Cachara.Shared.Infrastructure.Data.EF.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cachara.Data.Persistence.Configuration
{
    public class PostEntityTypeConfiguration : BaseEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Body)
                .HasMaxLength(4000)
                .IsRequired();
            
            builder.Property(t => t.AuthorId)
                .HasMaxLength(36)
                .IsRequired();
        }
    }
}
