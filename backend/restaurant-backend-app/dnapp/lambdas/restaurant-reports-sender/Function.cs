using System.Collections.Generic;
using Amazon.Lambda.Core;
using System;
using Function.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Function.DependencyInjection;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleLambdaFunction;

public class Function
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReportSenderService _reportSenderService;

    public Function()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        _serviceProvider = services.BuildServiceProvider();

        // Resolve the service
        _reportSenderService = _serviceProvider.GetRequiredService<IReportSenderService>();
    }

    public async Task FunctionHandler(Dictionary<string, object> eventData, ILambdaContext context)
    {
        try
        {
            await _reportSenderService.SendReportAsync(eventData);
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"Error occured while sending a report: {ex.Message}");
            throw;
        }
        context.Logger.LogLine("Report sent successfully.");
    }
}
