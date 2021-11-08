using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.Core.Tools.DatabaseInitialization;
using eShopWebApi.Helpers;
using eShopWebApi.Infrastructure.Data;
using eShopWebApi.Infrastructure.Data.Initialization;
using eShopWebApi.Middlewares;
using eShopWebApi.SwaggerConfigurationOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace eShopWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private bool ShouldUseInMemoryDb => Configuration.GetValue<bool>("eShopWebApi:UseInMemoryApplicationDb");


        // TODO: Cleanup services configuration. Separate into smaller extension methods in Configuration directory.
        public void ConfigureServices(IServiceCollection services)
        {
            if (ShouldUseInMemoryDb)
            {
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("eShopWebApiDatabase"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbConnection")));
            }

            services.AddScoped(typeof(IAsyncRepository<>), typeof(EntityFrameworkRepository<>));

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddSingleton<InitDataProvider<Product>, ProductsDataJsonProvider>();
            services.AddSingleton<IFileReader, FileReader>();

            services.AddScoped<IProductService, ProductService>();

            services.AddHttpContextAccessor();
            services.AddSingleton<IPagingConfigurationCalculator, PagingConfigurationCalculator>();
            services.AddScoped<IPagingHelper, PagingHelper>();

            services.AddControllers();

            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(setup => 
            {
                setup.OperationFilter<AddResponseHeadersFilter>();
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            services.ConfigureOptions<ConfigureSwaggerOptions>();

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            ILogger<Startup> logger,
            IDatabaseInitializer databaseInitializer, 
            IApiVersionDescriptionProvider apiVersionDescriptionProvider,
            ApplicationDbContext dbContext)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach(var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });


            ApplyMigrationsIfRequired(dbContext, logger);
            SeedDatabaseWithTestDataIfRequired(databaseInitializer, logger);
        }

        // TODO: Implement methods as extension methods on app.
        private void SeedDatabaseWithTestDataIfRequired(IDatabaseInitializer databaseInitializer, ILogger<Startup> logger)
        {
            try
            {
                if (Configuration.GetValue<bool>("eShopWebApi:SeedDatabaseWithTestData"))
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
            catch(Exception ex)
            {
                logger.LogError(ex, "Database initialization failed. Restart application to try it again.");
            }
        }

        private void ApplyMigrationsIfRequired(ApplicationDbContext context, ILogger<Startup> logger)
        {
            try
            {
                // Migration didn't work for in-memory database so this setting has to be disabled
                var applyMigrationIfNeeded = Configuration.GetValue<bool>("eShopWebApi:ApplyDbMigrationIfNeeded");
                if (!ShouldUseInMemoryDb && applyMigrationIfNeeded && context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                    logger.LogInformation("Database was migrated to the latest version. ShouldUseInMemoryDb={shouldUseInMemoryDb}, ApplyMigrationIfNeeded={applyMigrationIfNeeded}",
                        ShouldUseInMemoryDb, applyMigrationIfNeeded);
                }
                else
                {
                    logger.LogInformation("Database migration was skipped. ShouldUseInMemoryDb={shouldUseInMemoryDb}, ApplyMigrationIfNeeded={applyMigrationIfNeeded}",
                        ShouldUseInMemoryDb, applyMigrationIfNeeded);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Database migrations were not applied due to error. Database was not migrated. Restart application to try it again.");
            }
        }
    }
}
