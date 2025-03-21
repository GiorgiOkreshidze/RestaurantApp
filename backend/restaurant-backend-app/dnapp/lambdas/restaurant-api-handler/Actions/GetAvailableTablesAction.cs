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
using Amazon.Lambda.Core;

namespace Function.Actions
{
    public class GetAvailableTablesAction
    {
        private readonly IDynamoDBService _dynamoDBService;
        private ILambdaContext? _context;

        public GetAvailableTablesAction()
        {
            _dynamoDBService = new DynamoDBService();
        }

        public void SetContext(ILambdaContext context)
        {
            _context = context;
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

                var currentUtcTime = DateTime.UtcNow;
                var currentUtcDate = currentUtcTime.Date;
                request.QueryStringParameters.TryGetValue("time", out var time);

                int guests = 1;

                if (request.QueryStringParameters.TryGetValue("guests", out var guestsStr) && !int.TryParse(guestsStr, out guests))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Valid guests parameter is required" });
                }

                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Invalid date format. Use yyyy-MM-dd." });
                }

                if (parsedDate < currentUtcDate)
                {
                    return ActionUtils.FormatResponse(400, new { message = "Reservation date cannot be in the past." });
                }

                if (guests < 1 || guests > 10) // Assuming a reasonable guest limit
                {
                    return ActionUtils.FormatResponse(400, new { message = "Guests must be between 1 and 10." });
                }

                TimeSpan parsedTime = TimeSpan.Zero;
                if (!string.IsNullOrEmpty(time))
                {
                if (!TimeSpan.TryParseExact(time, "hh\\:mm", CultureInfo.InvariantCulture, out parsedTime))
                    {
                        return ActionUtils.FormatResponse(400, new { message = "Invalid time format. Use hh:mm." });
                    }

                    var reservationDateTime = parsedDate.Add(parsedTime);

                    if (reservationDateTime < currentUtcTime)
                    {
                        return ActionUtils.FormatResponse(400, new { message = "Reservation time cannot be in the past." });
                    }
                }

                _context?.Logger.LogInformation($"parameters: {guests}, {date}, {time}, {locationId},  ");

                var locationDetails = await _dynamoDBService.GetLocationDetails(locationId);

                _context?.Logger.LogInformation($"location: {locationDetails}  ");

                if (locationDetails == null)
                 {
                    return ActionUtils.FormatResponse(404, new { message = "Location not found" });
                }

                var tables = await _dynamoDBService.GetTablesForLocation(locationId, guests);
                _context?.Logger.LogInformation($"tables: {tables}  ");

                var reservations = await _dynamoDBService.GetReservationsForDateAndLocation(date, locationDetails.Address);

               _context?.Logger.LogInformation($"reservations: {reservations}  ");

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
            string? requestedTime)
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

     

        private List<TimeSlot> FilterAvailableTimeSlots(List<TimeSlot> allSlots, List<ReservationInfo> reservations)
        {
            var availableSlots = new List<TimeSlot>();

            foreach (var slot in allSlots)
            {
                var slotStartTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
                var slotEndTime = TimeSpan.ParseExact(slot.End, "hh\\:mm", CultureInfo.InvariantCulture);

                bool isAvailable = true;

                foreach (var reservation in reservations)
                {
                    var reservationStart = TimeSpan.ParseExact(reservation.TimeFrom, "hh\\:mm", CultureInfo.InvariantCulture);
                    var reservationEnd = TimeSpan.ParseExact(reservation.TimeTo, "hh\\:mm", CultureInfo.InvariantCulture);

                    if (slotStartTime < reservationEnd && slotEndTime > reservationStart)
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
                    return new List<TimeSlot> { slot };
                }
            }
            // If no exact match is found, check for the nearest slot within a 15-minute window
            var nearestSlot = availableSlots
                .Where(slot =>
                {
                    var slotTime = TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture);
                    var timeDifference = Math.Abs((slotTime - requestedTimeSpan).TotalMinutes);
                    return timeDifference <= 15; // Check if the slot is within 15 minutes of the requested time
                 })
                .OrderBy(slot => Math.Abs((TimeSpan.ParseExact(slot.Start, "hh\\:mm", CultureInfo.InvariantCulture) - requestedTimeSpan).Ticks))
                .FirstOrDefault();

    // If a nearest slot is found, return it
            if (nearestSlot != null)
            {
                return new List<TimeSlot> { nearestSlot };
            }
            // If no slot contains the requested time, return empty list
            return new List<TimeSlot>();
        }

        private List<TimeSlot> GeneratePredefinedTimeSlots()
        {
            var slots = new List<TimeSlot>();
            var startTimeUtc = new TimeSpan(6, 30, 0); // 6:30 AM UTC (10:30 AM Tbilisi time)
            var endTimeUtc = new TimeSpan(18, 30, 0);   // 6:30 PM UTC (10:30 PM Tbilisi time)

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
}
