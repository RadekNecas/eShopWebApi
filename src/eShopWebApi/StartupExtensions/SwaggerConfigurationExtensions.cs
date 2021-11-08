using eShopWebApi.SwaggerConfigurationOptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Reflection;

namespace eShopWebApi.StartupExtensions
{
    public static class SwaggerConfigurationExtensions
    {
        public static IApplicationBuilder UseSwaggerWithPaging(this IApplicationBuilder app, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            return app;
        }

        public static IServiceCollection AddSwaggerGenWithXmlCommentsAndVerioningSupport(this IServiceCollection services)
        {
            services.AddSwaggerGen(setup =>
            {
                setup.OperationFilter<AddResponseHeadersFilter>();
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                setup.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            services.ConfigureOptions<ConfigureSwaggerOptions>();

            return services;
        }
    }
}
