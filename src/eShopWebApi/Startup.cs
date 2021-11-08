using eShopWebApi.Core.Entities.Product;
using eShopWebApi.Core.RequiredExternalities;
using eShopWebApi.Core.Services;
using eShopWebApi.Core.Tools;
using eShopWebApi.Core.Tools.DatabaseInitialization;
using eShopWebApi.Helpers;
using eShopWebApi.Infrastructure.Data;
using eShopWebApi.Infrastructure.Data.Initialization;
using eShopWebApi.Middlewares;
using eShopWebApi.StartupExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShopWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // TODO: Cleanup services configuration. Separate into smaller extension methods in Configuration directory.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRequestedDbContext(Configuration);

            RegisterApplicationServices(services);

            services.AddHttpContextAccessor();

            services.AddControllers();

            services.AddConfiguredApiVersioning();
            services.AddSwaggerGenWithXmlCommentsAndVerioningSupport();
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

            app.UseSwaggerWithPaging(apiVersionDescriptionProvider);
            app.ApplyMigrationsIfRequired(dbContext, logger, Configuration);
            app.SeedDatabaseWithTestDataIfRequired(databaseInitializer, logger, Configuration);
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EntityFrameworkRepository<>));

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
            services.AddSingleton<InitDataProvider<Product>, ProductsDataJsonProvider>();
            services.AddSingleton<IFileReader, FileReader>();

            services.AddScoped<IProductService, ProductService>();

            services.AddSingleton<IPagingConfigurationCalculator, PagingConfigurationCalculator>();
            services.AddScoped<IPagingHelper, PagingHelper>();
        }
    }
}
