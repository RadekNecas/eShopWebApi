using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Services;
using eShopWebApi.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            services.AddScoped<IProductService, ProductService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, ApplicationDbContext dbContext)
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

            ApplyMigrationsIfNeeded(dbContext, logger);
        }


        private void ApplyMigrationsIfNeeded(ApplicationDbContext context, ILogger<Startup> logger)
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
