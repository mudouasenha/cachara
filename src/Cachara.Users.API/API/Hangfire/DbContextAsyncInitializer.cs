using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Users.API.API.Hangfire;

public class DbContextInitializer<TDbContext> : IAsyncInitializer where TDbContext : DbContext
{
    private readonly TDbContext dbContext;

    public DbContextInitializer(TDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // TODO: Fix Migrations not being applied on startup
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("Applying migrations...\n\n\n\n\n\n\n\n");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("Migrations applied successfully.");
    }
}
