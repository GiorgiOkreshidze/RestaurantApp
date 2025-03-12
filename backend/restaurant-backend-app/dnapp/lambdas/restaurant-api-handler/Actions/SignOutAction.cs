using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Function.Actions
{
    class SignOutAction
    {
        private readonly IAuthenticationService _authenticationService;

        public SignOutAction()
        {
            _authenticationService = new AuthenticationService();
        }

        public async Task<APIGatewayProxyResponse> Signout(APIGatewayProxyRequest request)
        {
            try
            {
                if (!request.Headers.TryGetValue("Authorization", out var accessToken) || string.IsNullOrWhiteSpace(accessToken))
                {
                    return new APIGatewayProxyResponse
                    {
                        StatusCode = 400,
                        Body = "{\"message\": \"Missing access token\"}",
                        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                    };
                }

                accessToken = accessToken.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

                await _authenticationService.SignOut(accessToken);

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "{\"message\": \"User signed out successfully\"}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (AuthenticationException ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 401,
                    Body = $"{{\"message\": \"{ex.Message}\"}}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
            catch (Exception ex)
            {
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"{{\"message\": \"Internal Server Error: {ex.Message}\"}}",
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
                };
            }
        }
    }
}
