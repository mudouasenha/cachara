using Extensions.Hosting.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cachara.Shared.Application;

public class DbContextInitializer<TDbContext>(TDbContext dbContext, ILogger<DbContextInitializer<TDbContext>> logger)
    : IAsyncInitializer
    where TDbContext : DbContext
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Applying migrations for {DbContext}...", typeof(TDbContext).Name);

            await dbContext.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("Migrations applied successfully for {DbContext}.", typeof(TDbContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying migrations for {DbContext}.", typeof(TDbContext).Name);
            throw;
        }
    }
}

