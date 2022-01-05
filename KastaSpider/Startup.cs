using Core.AutoMapperProfiles;
using Core.Configurations;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.Services;
using Services.Services.Implementations;
using Services.Workers;
using Services.Workers.Implementations;
using System;
using System.Collections.Generic;
using System.IO;

[assembly: FunctionsStartup(typeof(KastaSpider.Startup))]

namespace KastaSpider
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
            string appRootPath = builder.GetContext().ApplicationRootPath;

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(appRootPath, "Configuration", "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(appRootPath, "Configuration", $"appsettings.{envName}.json"), optional: true, reloadOnChange: true);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.GetContext().Configuration;

            // configurations
            services.Configure<MongoDBConfiguration>(configuration.GetSection(nameof(MongoDBConfiguration)));
            services.Configure<RabbitMQQueueConfiguration>(configuration.GetSection(nameof(RabbitMQQueueConfiguration)));
            services.Configure<KastaSpiderConfiguration>(configuration.GetSection(nameof(KastaSpiderConfiguration)));

            // databases
            services.AddSingleton<IMongoClient>(p => {
                string connectionString = p.GetRequiredService<IOptions<MongoDBConfiguration>>().Value.ConnectionString;
                return new MongoClient(connectionString);
            });
            services.AddScoped<IMongoDatabase>(p => {
                string databaseName = p.GetRequiredService<IOptions<MongoDBConfiguration>>().Value.DatabaseName;
                return p.GetRequiredService<IMongoClient>().GetDatabase(databaseName);
            });

            // services
            services.AddScoped<ILogService, MongoLogService>();
            services.AddScoped<IKastaClient, KastaClient>();
            services.AddScoped<IQueueService, RabbitMQQueueService>();
            services.AddScoped<IProductService, ProductService>();

            //workers
            services.AddScoped<IWorker<SitemapsFetcherWorker, object>, SitemapsFetcherWorker>();
            services.AddScoped<IWorker<ProductsFetcherWorker, IEnumerable<string>>, ProductsFetcherWorker>();
            services.AddScoped<IWorker<ProductsParserWorker, IEnumerable<string>>, ProductsParserWorker>();

            // automapper
            services.AddAutoMapper(typeof(KastaToDomainProfile));
        }
    }
}