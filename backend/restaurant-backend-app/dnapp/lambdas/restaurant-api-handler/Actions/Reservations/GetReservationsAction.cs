using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Mappers;
using Function.Models.Reservations;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Reservations;

public class GetReservationsAction
{
    private readonly IReservationService _reservationService;

    public GetReservationsAction()
    {
        _reservationService = new ReservationService();
    }

    public async Task<APIGatewayProxyResponse> GetReservationsAsync(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);

        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")!.Value;
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "custom:role")!.Value.ToRoles();
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")!.Value;

        var queryParams = ExtractReservationQueryParams(request);

        var reservations = await _reservationService.GetReservationsAsync(queryParams, userId, email, role);


        var response = Mapper.MapReservationsToReservationResponses(reservations);
        
        return ActionUtils.FormatResponse(200, response);
    }

    private ReservationsQueryParameters ExtractReservationQueryParams(APIGatewayProxyRequest request)
    {
        if(request.QueryStringParameters is null)
        {
            return new ReservationsQueryParameters();
        }

        var dateParam = request.QueryStringParameters.TryGetValue("date", out string? value) ? value : string.Empty;
        var timeFromParam = request.QueryStringParameters.TryGetValue("timeFrom", out string? timeFrom) ? timeFrom : string.Empty;
        var tableIdParam = request.QueryStringParameters.TryGetValue("tableId", out string? tableId) ? tableId : string.Empty;

        return new ReservationsQueryParameters
        {
            Date = dateParam,
            TimeFrom = timeFromParam,
            TableId = tableIdParam
        };
    }
}