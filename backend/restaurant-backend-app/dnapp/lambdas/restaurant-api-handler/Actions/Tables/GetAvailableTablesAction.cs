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
    private readonly ILocationService _locationService;
    private readonly IReservationService _reservationService;

    public GetAvailableTablesAction()
    {
        _tableService = new TableService();
        _locationService = new LocationService();
        _reservationService = new ReservationService();
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

        var locationDetails = await _locationService.GetLocationDetailsAsync(locationId);
        var tables = await _tableService.GetTablesForLocationAsync(locationId, guests);
        var reservations = await _reservationService.GetReservationsForDateAndLocation(date, locationDetails.Address);
        var result = CalculateAvailableSlots(tables, reservations, locationDetails, time);
        var tablesWithSlots = result.Where(t => t.AvailableSlots.Count != 0).ToList();

        return ActionUtils.FormatResponse(200, tablesWithSlots);
    }

    private List<TableResponse> CalculateAvailableSlots(
        List<RestaurantTable> tables,
        List<ReservationInfo> reservations,
        LocationInfo locationDetails,
        string? requestedTime)
    {
        var result = new List<TableResponse>();

        foreach (var table in tables)
        {
            var allTimeSlots = GeneratePredefinedTimeSlots();
            var tableReservations = reservations
                .Where(r => r.TableNumber == table.TableNumber)
                .ToList();
            var availableSlots = FilterAvailableTimeSlots(allTimeSlots, tableReservations);

            if (!string.IsNullOrEmpty(requestedTime))
            {
                availableSlots = FilterSlotsByRequestedTime(availableSlots, requestedTime);

                if (!availableSlots.Any())
                {
                    continue;
                }
            }

            result.Add(new TableResponse
            {
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                LocationId = table.LocationId,
                LocationAddress = table.LocationAddress,
                AvailableSlots = availableSlots
            });
        }

        return result;
    }

    private List<TimeSlot> FilterAvailableTimeSlots(List<TimeSlot> allSlots, List<ReservationInfo> reservations)
    {
        var availableSlots = new List<TimeSlot>();

        foreach (var slot in allSlots)
        {
            var slotStartTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
            var slotEndTime = TimeSpan.ParseExact(slot.End, "hh\\:mm", CultureInfo.InvariantCulture);

            var isAvailable = true;

            foreach (var reservation in reservations)
            {
                var reservationStart =
                    TimeSpan.ParseExact(reservation.TimeFrom, "hh\\:mm", CultureInfo.InvariantCulture);
                var reservationEnd = TimeSpan.ParseExact(reservation.TimeTo, "hh\\:mm", CultureInfo.InvariantCulture);

                if (slotStartTime >= reservationEnd || slotEndTime <= reservationStart) continue;
                isAvailable = false;
                break;
            }

            if (isAvailable)
            {
                availableSlots.Add(slot);
            }
        }

        return availableSlots;
    }


    private List<TimeSlot> FilterSlotsByRequestedTime(List<TimeSlot> availableSlots, string requestedTime)
    {
        var requestedTimeSpan = TimeSpan.ParseExact(requestedTime, "hh\\:mm", CultureInfo.InvariantCulture);

        foreach (var slot in availableSlots)
        {
            var slotTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
            var slotEndTime = TimeSpan.ParseExact(slot.End, "hh\\:mm", CultureInfo.InvariantCulture);

            // If the requested time falls within this slot's duration
            if (requestedTimeSpan >= slotTime && requestedTimeSpan <= slotEndTime)
            {
                // Return only this slot
                return [slot];
            }
        }

        // If no exact match is found, check for the nearest slot within a 15-minute window
        var nearestSlot = availableSlots
            .Where(slot =>
            {
                var slotTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
                var timeDifference = Math.Abs((slotTime - requestedTimeSpan).TotalMinutes);
                return timeDifference <= 15; // Check if the slot is within 15 minutes of the requested time
            }).MinBy(slot =>
                Math.Abs((TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture) - requestedTimeSpan)
                    .Ticks));

        // If the nearest slot is found, return it, // If no slot contains the requested time, return empty list
        return nearestSlot != null ? [nearestSlot] : [];
    }

    private List<TimeSlot> GeneratePredefinedTimeSlots()
    {
        var slots = new List<TimeSlot>();
        var startTimeUtc = new TimeSpan(6, 30, 0); // 6:30 AM UTC (10:30 AM Tbilisi time)
        var endTimeUtc = new TimeSpan(18, 30, 0); // 6:30 PM UTC (10:30 PM Tbilisi time)
        var currentTime = startTimeUtc;
        
        while (currentTime <= endTimeUtc)
        {
            var slotEnd = currentTime.Add(TimeSpan.FromMinutes(90));
            
            slots.Add(new TimeSlot
            {
                Start = currentTime.ToString(@"hh\:mm"),
                End = slotEnd.ToString(@"hh\:mm")
            });

            currentTime = currentTime.Add(TimeSpan.FromMinutes(90 + 15)); // 90-minute slot + 15-minute gap
        }

        return slots;
    }
}