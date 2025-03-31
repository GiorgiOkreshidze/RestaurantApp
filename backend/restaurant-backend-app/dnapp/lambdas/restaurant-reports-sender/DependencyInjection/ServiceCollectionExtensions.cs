using Amazon.DynamoDBv2;
using Amazon.SimpleEmail;
using Function.Repositories;
using Function.Repositories.Interfaces;
using Function.Services;
using Function.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Function.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IAmazonDynamoDB, AmazonDynamoDBClient>();
            services.AddSingleton<AmazonSimpleEmailServiceClient>();

            // Repositories
            services.AddSingleton<IReportRepository, ReportRepository>();

            // Services
            services.AddSingleton<IReportSenderService, ReportSenderService>();

            return services;
        }
    }
}