using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Actions.Validators;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class CreateClientReservationAction
{
    private readonly IReservationService _reservationService;

    public CreateClientReservationAction()
    {
        _reservationService = new ReservationService();
    }

    public async Task<APIGatewayProxyResponse> CreateReservationAsync(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;


        if (string.IsNullOrEmpty(userId))
        {
            return ActionUtils.FormatResponse(403, new { message = "Forbidden: Resource not found." });
        }

        var reservationRequest = JsonSerializer.Deserialize<ClientReservationRequest>(request.Body);
        
        if (reservationRequest == null)
        {
            throw new ArgumentException("Reservation request body was null");
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