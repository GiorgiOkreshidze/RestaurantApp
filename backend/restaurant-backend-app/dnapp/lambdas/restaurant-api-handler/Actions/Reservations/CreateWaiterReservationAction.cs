using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Actions.Validators;
using Function.Models.Requests;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class CreateWaiterReservationAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IReservationService _reservationService;
    private readonly IUserService _userService;

    public CreateWaiterReservationAction()
    {
        _authenticationService = new AuthenticationService();
        _reservationService = new ReservationService();
        _userService = new UserService();
    }
    
public async Task<APIGatewayProxyResponse> CreateReservationAsync(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;


        if (string.IsNullOrEmpty(userId))
        {
            return ActionUtils.FormatResponse(403, new { message = "Forbidden: Resource not found." });
        }

        var reservationRequest = JsonSerializer.Deserialize<WaiterReservationRequest>(request.Body);

        if (reservationRequest == null)
        {
            throw new ArgumentException("Reservation request body was null");
        }
   
        // Create operation
        if (reservationRequest.ClientType is not ClientType.CUSTOMER and not ClientType.VISITOR)
        {
            throw new ArgumentException("ClientType must be either CUSTOMER or VISITOR");
        }

        if (reservationRequest.ClientType == ClientType.CUSTOMER && string.IsNullOrEmpty(reservationRequest.CustomerName))
        {
            throw new ArgumentException("CustomerName is required for CUSTOMER type reservations");
        }

        ReservationValidator.ValidateGuestsNumber(reservationRequest.GuestsNumber);
        
        var parsedDate = ReservationValidator.ParseDate(reservationRequest.Date);
        var parsedStartTime = ReservationValidator.ParseTime(reservationRequest.TimeFrom, "TimeFrom");
        var parsedEndTime = ReservationValidator.ParseTime(reservationRequest.TimeTo, "TimeTo");
        var reservationDateTimeFrom = parsedDate.Add(parsedStartTime);
        var reservationDateTimeTo = parsedDate.Add(parsedEndTime);

        ReservationValidator.ValidateFutureDateTime(reservationDateTimeFrom);
        ReservationValidator.ValidateFutureDateTime(reservationDateTimeTo);

        var reservationResponse = await _reservationService.UpsertReservationAsync(reservationRequest, userId);
        return ActionUtils.FormatResponse(200, reservationResponse);
    }
}