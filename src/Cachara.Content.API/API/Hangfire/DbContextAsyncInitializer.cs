using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Content.API.API.Hangfire;

public class DbContextInitializer<TDbContext> : IAsyncInitializer where TDbContext : DbContext
{
    private readonly TDbContext dbContext;

    public DbContextInitializer(TDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await dbContext.Database.MigrateAsync();
    }
}