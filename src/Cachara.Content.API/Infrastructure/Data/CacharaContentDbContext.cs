﻿using System.Data;
using Cachara.Data.Interfaces;
using Cachara.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cachara.Content.API.Infrastructure.Data
{
    public class CacharaContentDbContext : DbContext, IUnitOfWork
    {
        private const string Schema = "Content";
        public CacharaContentDbContext(DbContextOptions<CacharaContentDbContext> options) : base(options)
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