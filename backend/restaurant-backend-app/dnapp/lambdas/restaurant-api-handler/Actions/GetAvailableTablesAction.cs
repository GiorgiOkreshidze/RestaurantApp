using Function.Services.Interfaces;
using Function.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Actions;
using Function.Models;
using System.Globalization;

namespace Function.Actions
{
    public class GetAvailableTablesAction
    {
        private readonly IDynamoDBService _dynamoDBService;

        public GetAvailableTablesAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public async Task<APIGatewayProxyResponse> GetAvailableTables(APIGatewayProxyRequest request)
        {
            try
            {
                if (!request.QueryStringParameters.TryGetValue("locationId", out var locationId))
                {
                    return ActionUtils.FormatResponse(400, new { message = "LocationId parameter is required" });
                }

                if (!request.QueryStringParameters.TryGetValue("date", out var date))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Date parameter is required" });
                }

                if (!request.QueryStringParameters.TryGetValue("time", out var time))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Time parameter is required" });
                }

                int guests = 1;

                if (request.QueryStringParameters.TryGetValue("guests", out var guestsStr) && !int.TryParse(guestsStr, out guests))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Valid guests parameter is required" });
                }

                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Invalid date format. Use yyyy-MM-dd." });
                }

                if (guests < 1 || guests > 20) // Assuming a reasonable guest limit
                {
                    return ActionUtils.FormatResponse(400, new { message = "Guests must be between 1 and 20." });
                }

                if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out var parsedTime))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Invalid time format. Use hh:mm." });
                }

                var locationDetails = await _dynamoDBService.GetLocationDetails(locationId);

                 if (locationDetails == null)
                 {
                    return ActionUtils.FormatResponse(404, new { message = "Location not found" });
                }

                var tables = await _dynamoDBService.GetTablesForLocation(locationId, guests);

                var reservations = await _dynamoDBService.GetReservationsForDateAndLocation(date, locationDetails.Address);

                var result = CalculateAvailableSlots(tables, reservations, locationDetails, time);

                var tablesWithSlots = result.Where(t => t.AvailableSlots.Any()).ToList();

                return ActionUtils.FormatResponse(200, tablesWithSlots);
            }
            catch (Exception e)
            {
                return ActionUtils.FormatResponse(500, new
                {
                    message = e.Message
                });
            }
        }


        private List<TableResponse> CalculateAvailableSlots(
    List<RestaurantTable> tables,
    List<ReservationInfo> reservations,
    LocationInfo locationDetails,
    string requestedTime)
        {
            var result = new List<TableResponse>();

            foreach (var table in tables)
            {

                // Get all standard time slots for this location
                var allTimeSlots = GeneratePredefinedTimeSlots();

                // Find reservations for this specific table
                var tableReservations = reservations
                    .Where(r => r.TableNumber == table.TableNumber)
                    .ToList();

                // Remove booked time slots
                var availableSlots = FilterAvailableTimeSlots(allTimeSlots, tableReservations);

                // If a specific time was requested, filter to only show the containing slot
                if (!string.IsNullOrEmpty(requestedTime))
                {
                    availableSlots = FilterSlotsByRequestedTime(availableSlots, requestedTime);

                    // If no slots match the requested time, skip this table
                    if (!availableSlots.Any())
                    {
                        continue;
                    }
                }

                // Create response object
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

     

        private List<string> FilterAvailableTimeSlots(List<string> allSlots, List<ReservationInfo> reservations)
        {
            var availableSlots = new List<string>();

            foreach (var slot in allSlots)
            {
                var slotTime = TimeSpan.ParseExact(slot, "hh\\:mm", CultureInfo.InvariantCulture);
                var slotEndTime = slotTime.Add(TimeSpan.FromMinutes(90));
                bool isAvailable = true;

                foreach (var reservation in reservations)
                {
                    var reservationStart = TimeSpan.ParseExact(reservation.TimeFrom, "hh\\:mm", CultureInfo.InvariantCulture);
                    var reservationEnd = TimeSpan.ParseExact(reservation.TimeTo, "hh\\:mm", CultureInfo.InvariantCulture);

                    if (slotTime < reservationEnd && slotEndTime > reservationStart)
                    {
                        isAvailable = false;
                        break;
                    }
                }

                if (isAvailable)
                {
                    availableSlots.Add(slot);
                }
            }

            return availableSlots;
        }


        private List<string> FilterSlotsByRequestedTime(List<string> availableSlots, string requestedTime)
        {
            var requestedTimeSpan = TimeSpan.ParseExact(requestedTime, "hh\\:mm", CultureInfo.InvariantCulture);

            foreach (var slot in availableSlots)
            {
                var slotTime = TimeSpan.ParseExact(slot, "hh\\:mm", CultureInfo.InvariantCulture);
                var slotEndTime = slotTime.Add(TimeSpan.FromMinutes(90));

                // If the requested time falls within this slot's duration
                if (requestedTimeSpan >= slotTime && requestedTimeSpan <= slotEndTime)
                {
                    // Return only this slot
                    return new List<string> { slot };
                }
            }

            // If no slot contains the requested time, return empty list
            return new List<string>();
        }

        private List<string> GeneratePredefinedTimeSlots()
        {
            var slots = new List<string>();
            var startTime = new TimeSpan(10, 30, 0); // 10:30 AM
            var endTime = new TimeSpan(22, 30, 0);   // 10:30 PM

            while (startTime <= endTime)
            {
                slots.Add(startTime.ToString(@"hh\:mm"));
                startTime = startTime.Add(TimeSpan.FromMinutes(90 + 15)); // 90-minute slot + 15-minute gap
            }

            return slots;
        }
    }
}
