using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class CreateReservationAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IReservationService _reservationService;
    private readonly IUserService _userService;

    public CreateReservationAction()
    {
        _authenticationService = new AuthenticationService();
        _reservationService = new ReservationService();
        _userService = new UserService();
    }

    public async Task<APIGatewayProxyResponse> CreateReservationAsync(APIGatewayProxyRequest request)
    {
        var accessToken = ActionUtils.GetAccessToken(request);
        var userInfo = await _authenticationService.GetUserDetailsAsync(accessToken);
        var reservationRequest = JsonSerializer.Deserialize<ClientReservationRequest>(request.Body);
        var email = userInfo.GetValueOrDefault("email");
        
        if (reservationRequest == null)
        {
            throw new ArgumentException("Reservation request body was null");
        }

        if (email == null)
        {
            throw new UnauthorizedException("User is not registered");
        }
        
        var user = await _userService.GetUserByEmailAsync(email);

        if (int.TryParse(reservationRequest.GuestsNumber, out var guests) && guests > 10)
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
        
        var reservationResponse = await _reservationService.UpsertReservationAsync(reservationRequest, user);

        return ActionUtils.FormatResponse(200, reservationResponse);
    }
}