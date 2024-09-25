using Cachara.Data.Interfaces;
using Cachara.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cachara.Data.EF
{
    public class CacharaSocialDbContext : DbContext, IUnitOfWork
    {
        private const string Schema = "Social";
        public CacharaSocialDbContext(DbContextOptions<CacharaSocialDbContext> options) : base(options)
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
