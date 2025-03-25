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
        request.Headers.TryGetValue("Authorization", out var idToken);

        if (string.IsNullOrEmpty(idToken) || !idToken.StartsWith("Bearer "))
        {
            throw new ArgumentException("Authorization header empty");
        }

        var token = idToken.Substring("Bearer ".Length).Trim();
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid token");
        }

        var jwtToken = tokenHandler.ReadJwtToken(token);
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")!.Value;
        var givenName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")!.Value;
        var familyName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")!.Value;
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "custom:role")!.Value.ToRoles();

        var userInfo = CreateUserInfo(familyName, givenName, email);

        var queryParams = ExtractReservationQueryParams(request);

        var reservations = await _reservationService.GetReservationsAsync(queryParams, userInfo, role);


        var response = Mapper.MapReservationsToReservationResponses(reservations);
        
        return ActionUtils.FormatResponse(200, response);
    }

    private string CreateUserInfo(string familyName, string givenName, string email)
    {
        var userFirstName = givenName;
        var userLastName = familyName;
        var userEmail = email;

        return $"{userFirstName} {userLastName}, {userEmail}";
    }

    private ReservationsQueryParameters ExtractReservationQueryParams(APIGatewayProxyRequest request)
    {
        if(request.QueryStringParameters is null)
        {
            return new ReservationsQueryParameters();
        }

        var dateParam = request.QueryStringParameters.TryGetValue("date", out string? value) ? value : string.Empty;
        var timeFromParam = request.QueryStringParameters.TryGetValue("timeFrom", out string? timeFrom) ? timeFrom : string.Empty;
        var tableNumberParam = request.QueryStringParameters.TryGetValue("tableNumber", out string? tableNumber) ? tableNumber : string.Empty;

        return new ReservationsQueryParameters
        {
            Date = dateParam,
            TimeFrom = timeFromParam,
            TableNumber = tableNumberParam
        };
    }
}