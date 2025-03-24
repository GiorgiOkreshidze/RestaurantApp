using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
using Function.Models.Reservations;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Tables;

public class GetAvailableTablesAction
{
    private readonly ITableService _tableService;

    public GetAvailableTablesAction()
    {
        _tableService = new TableService();
    }

    public async Task<APIGatewayProxyResponse> GetAvailableTables(APIGatewayProxyRequest request)
    {
        if (!request.QueryStringParameters.TryGetValue("locationId", out var locationId))
        {
            throw new ArgumentException("LocationId parameter is required");
        }

        if (!request.QueryStringParameters.TryGetValue("date", out var date))
        {
            throw new ArgumentException("Date parameter is required");
        }

        var currentUtcTime = DateTime.UtcNow;
        var currentUtcDate = currentUtcTime.Date;

        request.QueryStringParameters.TryGetValue("time", out var time);

        int guests = 1;

        if (request.QueryStringParameters.TryGetValue("guests", out var guestsStr) &&
            !int.TryParse(guestsStr, out guests))
        {
            throw new ArgumentException("Valid guests parameter is required");
        }

        if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var parsedDate))
        {
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
        }

        if (parsedDate < currentUtcDate)
        {
            throw new ArgumentException("Reservation date cannot be in the past.");
        }

        if (guests < 1 || guests > 10)
        {
            throw new ArgumentException("Guests must be between 1 and 10.");
        }

        if (!string.IsNullOrEmpty(time))
        {
            if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out var parsedTime))
            {
                throw new ArgumentException("Invalid time format. Use hh:mm.");
            }

            var reservationDateTime = parsedDate.Add(parsedTime);

            if (reservationDateTime < currentUtcTime)
            {
                throw new ArgumentException("Reservation time cannot be in the past.");
            }
        }

        var tablesWithSlots = await _tableService.GetAvailableTablesAsync(locationId, date, time, guests);

        return ActionUtils.FormatResponse(200, tablesWithSlots);
    }
}