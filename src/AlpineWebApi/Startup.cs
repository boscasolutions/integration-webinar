using AlpineWebApi.Data;
using AlpineWebApi.Data.Repositories;
using AlpineWebApi.Data.Repositories.Repositories.Interfaces;
using AlpineWebApi.Data.Services;
using AlpineWebApi.Data.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlpineWebApi
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

            services.AddControllers();

            services.AddSwaggerGen();
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
            string databaseName = "OrderShippingDb";

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

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}