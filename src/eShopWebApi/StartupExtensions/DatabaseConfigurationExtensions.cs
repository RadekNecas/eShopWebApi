using eShopWebApi.Core.Tools;
using eShopWebApi.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace eShopWebApi.StartupExtensions
{
    public static class DatabaseConfigurationExtensions
    {
        private static bool ShouldUseInMemoryDb(IConfiguration configuration)
            => configuration.GetValue<bool>("eShopWebApi:UseInMemoryApplicationDb");

        public static IServiceCollection AddRequestedDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            if (ShouldUseInMemoryDb(configuration))
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("eShopWebApiDatabase"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("ApplicationDbConnection")));
            }

            return services;
        }

        public static IApplicationBuilder SeedDatabaseWithTestDataIfRequired(this IApplicationBuilder app, IDatabaseInitializer databaseInitializer, ILogger<Startup> logger, IConfiguration configuration)
        {
            try
            {
                if (configuration.GetValue<bool>("eShopWebApi:SeedDatabaseWithTestData"))
                {
                    logger.LogInformation("Database initialization with test data required.");
                    if (databaseInitializer.GetExistingRecordsCount() == 0)
                    {
                        databaseInitializer.InitializeDatabase();
                        logger.LogInformation("Database was initialized with test data");
                    }
                    else
                    {
                        logger.LogInformation("Database already contains data. Initialization was skipped.");
                    }
                }
                else
                {
                    logger.LogInformation("Database initialization with test data was not required. Initialization skipped.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed. Restart application to try it again.");
            }

            return app;
        }

        public static IApplicationBuilder ApplyMigrationsIfRequired(this IApplicationBuilder app, ApplicationDbContext context, ILogger<Startup> logger, IConfiguration configuration)
        {
            try
            {
                // Migration didn't work for in-memory database so this setting has to be disabled
                var applyMigrationIfNeeded = configuration.GetValue<bool>("eShopWebApi:ApplyDbMigrationIfNeeded");
                if (!ShouldUseInMemoryDb(configuration) && applyMigrationIfNeeded && context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                    logger.LogInformation("Database was migrated to the latest version. ShouldUseInMemoryDb={shouldUseInMemoryDb}, ApplyMigrationIfNeeded={applyMigrationIfNeeded}",
                        ShouldUseInMemoryDb(configuration), applyMigrationIfNeeded);
                }
                else
                {
                    logger.LogInformation("Database migration was skipped. ShouldUseInMemoryDb={shouldUseInMemoryDb}, ApplyMigrationIfNeeded={applyMigrationIfNeeded}",
                        ShouldUseInMemoryDb(configuration), applyMigrationIfNeeded);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migrations were not applied due to error. Database was not migrated. Restart application to try it again.");
            }

            return app;
        }
    }
}
