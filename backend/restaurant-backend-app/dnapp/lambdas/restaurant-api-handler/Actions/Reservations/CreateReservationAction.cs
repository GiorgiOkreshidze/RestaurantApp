using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class CreateReservationAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILocationService _locationService;
    private readonly IReservationService _reservationService;

    public CreateReservationAction()
    {
        _authenticationService = new AuthenticationService();
        _locationService = new LocationService();
        _reservationService = new ReservationService();
    }

    public async Task<APIGatewayProxyResponse> CreateReservationAsync(APIGatewayProxyRequest request)
    {
        var accessToken = ActionUtils.GetAccessToken(request);
        var userInfo = await _authenticationService.GetUserDetailsAsync(accessToken);
        var reservationRequest = JsonSerializer.Deserialize<ReservationRequest>(request.Body);
        var firstName = userInfo.GetValueOrDefault("given_name");
        var lastName = userInfo.GetValueOrDefault("family_name");
        var fullName = $"{firstName} {lastName}";
        
        if (reservationRequest == null)
        {
            throw new ArgumentException("Reservation request body was null");
        }

        if (int.TryParse(reservationRequest.GuestsNumber, out int guests) && guests > 10)
        {
            throw new ArgumentException("The maximum number of guests allowed is 10");
        }

        if (string.IsNullOrEmpty(reservationRequest.Date))
        {
            throw new ArgumentException("Date is required.");
        }

        if (string.IsNullOrEmpty(reservationRequest.TimeFrom))
        {
            throw new ArgumentException("TimeFrom is required.");
        }

        if (string.IsNullOrEmpty(reservationRequest.TimeTo))
        {
            throw new ArgumentException("TimeTo is required.");
        }

        if (!DateTime.TryParseExact(reservationRequest.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
        }

        if (!TimeSpan.TryParseExact(reservationRequest.TimeFrom, "hh\\:mm", CultureInfo.InvariantCulture,
                out var parsedStartTime))
        {
            throw new ArgumentException("Invalid time format. Use hh:mm.");
        }

        if (!TimeSpan.TryParseExact(reservationRequest.TimeTo, "hh\\:mm", CultureInfo.InvariantCulture,
                out var parsedEndTime))
        {
            throw new ArgumentException("Invalid time format. Use hh:mm.");
        }

        var reservationDateTimeFrom = parsedDate.Add(parsedStartTime);
        var reservationDateTimeTo = parsedDate.Add(parsedStartTime);

        var currentUtcTime = DateTime.UtcNow;

        if (reservationDateTimeFrom < currentUtcTime || reservationDateTimeTo < currentUtcTime)
        {
            throw new ArgumentException("Reservation date and time must be in the future.");
        }

        var location = await _locationService.GetLocationByIdAsync(reservationRequest.LocationId);

        var existingReservations = await _reservationService.GetReservationsByDateLocationTable(
            reservationRequest.Date,
            location.Address,
            reservationRequest.TableNumber);

        var newTimeFrom = TimeSpan.Parse(reservationRequest.TimeFrom);
        var newTimeTo = TimeSpan.Parse(reservationRequest.TimeTo);

        foreach (var existingReservation in existingReservations)
        {
            var existingTimeFrom = TimeSpan.Parse(existingReservation.TimeFrom);
            var existingTimeTo = TimeSpan.Parse(existingReservation.TimeTo);

            // Check if the time slots overlap
            if (newTimeFrom < existingTimeTo &&
                newTimeTo > existingTimeFrom &&
                existingReservation.UserInfo != fullName)
            {
                throw new ArgumentException(
                    $"Table #{reservationRequest.TableNumber} at location " +
                    $"{location.Address} is already booked during the requested time period."
                );
            }
        }

        var reservationId = Guid.NewGuid().ToString();
        
        if (reservationRequest.Id != null)
        {
            reservationId = reservationRequest.Id;
        }

        var reservation = new Reservation
        {
            Id = reservationId,
            Date = reservationRequest.Date,
            FeedbackId = "NOT IMPLEMENTED YET AND ISN'T REQUIRED",
            GuestsNumber = reservationRequest.GuestsNumber,
            LocationAddress = location.Address,
            PreOrder = "NOT IMPLEMENTED YET",
            Status = Status.Reserved.ToString(),
            TableNumber = reservationRequest.TableNumber,
            TimeFrom = reservationRequest.TimeFrom,
            TimeTo = reservationRequest.TimeTo,
            TimeSlot = reservationRequest.TimeFrom + " - " + reservationRequest.TimeTo,
            UserInfo = fullName,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var reservationResponse = await _reservationService.UpsertReservationAsync(reservation);

        return ActionUtils.FormatResponse(200, reservationResponse);
    }
}