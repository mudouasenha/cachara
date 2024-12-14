using Microsoft.EntityFrameworkCore;
using Cachara.Shared.Infrastructure.Data.Interfaces;

namespace Cachara.Data.EF
{
    public class CacharaUsersDbContext : DbContext, IUnitOfWork
    {
        private const string Schema = "Users";
        public CacharaUsersDbContext(DbContextOptions<CacharaUsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
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
    }
}
