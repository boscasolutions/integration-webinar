using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TestApi.Data.Services;
using TestApi.Data.Services.Interfaces;
using TestApi.Data.Repositories.Repositories.Interfaces;
using TestApi.Data.Repositories;
using TestApi.Data;
using Microsoft.EntityFrameworkCore;

namespace TestApi
{
    public class Startup
    {
        private readonly IConfigurationRoot m_configuration;

        public Startup(IWebHostEnvironment env)
        {
            m_configuration = new ConfigurationBuilder()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(m_configuration.GetSection("Logging"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("Development", builder => builder
                    .WithMethods("GET", "POST", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type"));
            });

            ConfigureTransientServices(services);
            ConfigureRepositories(services);
            ConfigureEntityFramework(services);

            services
                .AddSwaggerGen()// Register the Swagger generator, defining 1 or more Swagger documents
                .AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        private static void ConfigureTransientServices(IServiceCollection services)
        {
            services.AddTransient<IOrderShippingService, OrderShippingService>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddSingleton<IOrderShippingRepository, OrderShippingRepository>();
        }

        private static void ConfigureEntityFramework(IServiceCollection services)
        {
            var databaseName = "OrderShippingDb";

            services.AddDbContext<OrderShippingDatabaseContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == Microsoft.Extensions.Hosting.Environments.Development)
            {
                app.UseCors("Development");
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
                app.UseCors("Production");
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{orderId?}");
            });
        }
    }
}