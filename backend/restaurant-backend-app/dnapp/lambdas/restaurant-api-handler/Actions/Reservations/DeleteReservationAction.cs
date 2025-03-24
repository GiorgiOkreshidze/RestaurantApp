using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

class DeleteReservationAction
{
    private readonly IReservationService _reservationService;

    public DeleteReservationAction()
    {
        _reservationService = new ReservationService();
    }

    public async Task<APIGatewayProxyResponse> DeleteReservationAsync(APIGatewayProxyRequest request)
    {
        string reservationId;

        try
        {
            reservationId = request.PathParameters["id"];
        }
        catch (Exception)
        {
            throw new ArgumentException("Reservation id is required.");
        }

        await _reservationService.CancelReservationAsync(reservationId);
        return ActionUtils.FormatResponse(200, "Reservation cancelled successfuly");
    }
}