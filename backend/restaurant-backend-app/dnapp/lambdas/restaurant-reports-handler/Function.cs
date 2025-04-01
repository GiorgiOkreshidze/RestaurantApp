using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.SQSEvents;
using System.Threading.Tasks;
using System.Text.Json;
using Function.Models;
using System;
using Function.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Function.DependencyInjection;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SimpleLambdaFunction;

public class Function
{
    private static readonly Dictionary<string, Type> EventTypeMapping = new()
    {
        { "reservation", typeof(Reservation) },
        // Add more event types and their payload types as needed
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly IReportService _reportService;

    public Function()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        _serviceProvider = services.BuildServiceProvider();

        // Resolve the service
        _reportService = _serviceProvider.GetRequiredService<IReportService>();

    }

    public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
    {
        foreach (var record in sqsEvent.Records)
        {
            try
            {
                var eventWrapper = JsonSerializer.Deserialize<EventWrapper>(record.Body);

                if (eventWrapper == null || string.IsNullOrEmpty(eventWrapper.EventType))
                {
                    context.Logger.LogLine("Invalid event: Missing eventType.");
                    continue;
                }

                if (!EventTypeMapping.TryGetValue(eventWrapper.EventType, out var payloadType))
                {
                    context.Logger.LogLine($"Unknown event type: {eventWrapper.EventType}");
                    continue;
                }

                var payloadJson = eventWrapper.Payload.GetRawText();
                var payload = JsonSerializer.Deserialize(payloadJson, payloadType);

                switch (payload)
                {
                    case Reservation reservation:
                        context.Logger.LogLine($"Reservation ID: {reservation.Id}");
                        context.Logger.LogLine($"Waiter ID: {reservation.WaiterId}");
                        await _reportService.ProcessReservationAsync(reservation);
                        break;
                    default:
                        context.Logger.LogLine($"Unhandled payload type for event: {eventWrapper.EventType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error processing event: {ex.Message}");
            }
        }
    }
}
