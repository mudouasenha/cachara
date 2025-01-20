namespace Cachara.Users.API.Infrastructure.Data.Configuration;

/*public class UserFollowerEntityTypeConfiguration : BaseEntityTypeConfiguration<UserFollower>
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

        // builder
        //     .HasOne(p => p.User)
        //     .WithMany(p => p.Followers)
        //     .HasForeignKey(p => p.UserId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // builder
        //     .HasOne(p => p.Follower)
        //     .WithMany(p => p.Following)
        //     .HasForeignKey(p => p.FollowerId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}*/
