using Cachara.Shared.Infrastructure.Data.Interfaces;
using Cachara.Users.API.Domain.Entities;
using Cachara.Users.API.Infrastructure.Data.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cachara.Users.API.Infrastructure.Data;

public class CacharaUsersDbContext : DbContext, IUnitOfWork
{
    private const string Schema = "Users";

    public CacharaUsersDbContext(DbContextOptions<CacharaUsersDbContext> options) : base(options)
    {
    }

    public Task<int> Commit()
    {
        return SaveChangesAsync();
    }

    public Task Discard()
    {
        ChangeTracker.Clear();
        return Task.CompletedTask;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(RoleSeeds.GetUserRoles());

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public static void ConfigureDBContext(SqlServerDbContextOptionsBuilder obj)
    {
        obj.MigrationsHistoryTable("__EFMigrationsHistory", Schema);
    }
}
