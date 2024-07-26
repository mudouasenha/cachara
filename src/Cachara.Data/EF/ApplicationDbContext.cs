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
    public class ApplicationContext : DbContext, IApplicationContext
    {
        private const string StaticSchema = "Social";
        private string Schema => StaticSchema;
        public DbSet<Post> Posts => Set<Post>();

        public IDbConnection Connection => Database.GetDbConnection();

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            builder.HasDefaultSchema(Schema);

            base.OnModelCreating(builder);
        }
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
