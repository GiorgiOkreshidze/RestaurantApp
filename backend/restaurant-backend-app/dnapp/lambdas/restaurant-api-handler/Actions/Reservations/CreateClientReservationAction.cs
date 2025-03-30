using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Actions.Validators;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class CreateClientReservationAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IReservationService _reservationService;
    private readonly IUserService _userService;

    public CreateClientReservationAction()
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

        ReservationValidator.ValidateGuestsNumber(reservationRequest.GuestsNumber);
        
        var parsedDate = ReservationValidator.ParseDate(reservationRequest.Date);
        var parsedStartTime = ReservationValidator.ParseTime(reservationRequest.TimeFrom, "TimeFrom");
        var parsedEndTime = ReservationValidator.ParseTime(reservationRequest.TimeTo, "TimeTo");
        var reservationDateTimeFrom = parsedDate.Add(parsedStartTime);
        var reservationDateTimeTo = parsedDate.Add(parsedEndTime);
        
        ReservationValidator.ValidateFutureDateTime(reservationDateTimeFrom);
        ReservationValidator.ValidateFutureDateTime(reservationDateTimeTo);
        
        var reservationResponse = await _reservationService.UpsertReservationAsync(reservationRequest, user);

        return ActionUtils.FormatResponse(200, reservationResponse);
    }
}