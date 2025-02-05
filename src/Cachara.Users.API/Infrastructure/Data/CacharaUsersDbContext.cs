﻿using Cachara.Shared.Infrastructure.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
