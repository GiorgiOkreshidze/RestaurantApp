using Amazon.Lambda.APIGatewayEvents;
using Function.Mappers;
using Function.Services;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Actions;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Actions
{
    public class GetReservationsAction
    {
        private readonly IDynamoDBService _dynamoDBService;
        private readonly IAuthenticationService _authenticationService;

        public GetReservationsAction()
        {
            _dynamoDBService = new DynamoDBService();
            _authenticationService = new AuthenticationService();
        }

        public async Task<APIGatewayProxyResponse> GetReservations(APIGatewayProxyRequest request)
        {
            try
            {
                request.Headers.TryGetValue("Authorization", out var IdToken);

                if (string.IsNullOrEmpty(IdToken) || !IdToken.StartsWith("Bearer "))
                {
                    return ActionUtils.FormatResponse(400, "Authorization header empty");
                }

                var token = IdToken.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();

                if (!tokenHandler.CanReadToken(token))
                {
                    return ActionUtils.FormatResponse(400, "Invalid Token");
                }

                var jwtToken = tokenHandler.ReadJwtToken(token);

                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value!;
                var givenName = jwtToken.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value!;
                var familyName = jwtToken.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value!;

                var userInfo = CreateUserInfo(familyName, givenName, email);

                var reservations = await _dynamoDBService.GetCustomerReservationsAsync(userInfo);

                var response = Mapper.MapReservationsToReservationResponses(reservations);
                return ActionUtils.FormatResponse(200, response);
            }
            catch (Exception e)
            {
                return ActionUtils.FormatResponse(500, new
                {
                    message = e.Message
                });
            }
        }

        private string CreateUserInfo(string familyName, string givenName, string email)
        {
            var userFirstName = givenName;
            var userLastName = familyName;
            var userEmail = email;

            return $"{userFirstName} {userLastName}, {userEmail}";
        }
    }
}
