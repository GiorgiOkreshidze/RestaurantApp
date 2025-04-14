using Amazon.DynamoDBv2;
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

            // Repositories
            services.AddSingleton<IEmployeeInfoRepository, EmployeeInfoRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<IFeedbacksRepository, FeedbacksRepository>();

            // Services
            services.AddSingleton<IReportService, ReportService>();

            return services;
        }
    }
}
