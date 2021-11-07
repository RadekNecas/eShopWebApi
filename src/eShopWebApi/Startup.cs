using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.Core.Tools.DatabaseInitialization;
using eShopWebApi.Helpers;
using eShopWebApi.Infrastructure.Data;
using eShopWebApi.Infrastructure.Data.Initialization;
using eShopWebApi.SwaggerConfigurationOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Filters;
using System.Linq;

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


        // This method gets called by the runtime. Use this method to add services to the container.
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
            if (env.IsDevelopment())
            {
                logger.LogInformation("Developer exception page will be used.");
                app.UseDeveloperExceptionPage();
            }

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

        private void SeedDatabaseWithTestDataIfRequired(IDatabaseInitializer databaseInitializer, ILogger<Startup> logger)
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

        private void ApplyMigrationsIfRequired(ApplicationDbContext context, ILogger<Startup> logger)
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
    }
}
