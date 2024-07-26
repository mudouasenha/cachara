using Cachara.API.Hangfire;
using Cachara.API.Options;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;

namespace Cachara.API.Extensions
{
    public static class HangFireExtensions
    {
        public static CacharaOptions Options { get; set; }

        public static IServiceCollection AddCustomHangfire(this IServiceCollection services)
        {
            services.AddScoped<IBackgroundServiceManager, BackgroundServiceManager>();

            services.AddHangfire((provider, config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
                config.UseRecommendedSerializerSettings(x =>
                {
                    //x.Converters.Add(new JsonDateOnlyConverter());
                });
                config.UseSqlServerStorage(Options.JobsSqlDb, new SqlServerStorageOptions()
                {
                    SchemaName = "BillsHangfire",
                });
                config.UseConsole();
            });

            //default HF value = Environment.ProcessorCount * 5
            //Prioridade de tasks nas queues com base no order do nome delas

            int totalWorkerCount = System.Environment.ProcessorCount * 20;

            int integrationWorkers = Convert.ToInt32(Math.Round(totalWorkerCount * 0.8)); //80% do total de workers
            int otherWorkers = Convert.ToInt32(Math.Round(totalWorkerCount * 0.2)); //20% do total de workers

            if (integrationWorkers > 0)
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = integrationWorkers;
                    options.Queues = new string[]
                    {
                        "integration",
                    };
                });
            }

            if (otherWorkers > 0)
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = otherWorkers;
                    options.Queues = new string[]
                    {
                        "default"
                    };
                });
            }

            return services;
        }
    }
}
