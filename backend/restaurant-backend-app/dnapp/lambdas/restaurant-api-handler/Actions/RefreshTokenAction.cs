using SimpleLambdaFunction.Services.Interfaces;
using SimpleLambdaFunction.Services;
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models;
using System.Text.Json;
using SimpleLambdaFunction.Actions;
using System.Security.Authentication;

namespace Function.Actions
{
    public class RefreshTokenAction
    {
        private readonly IAuthenticationService _authenticationService;

        public RefreshTokenAction()
        {
            _authenticationService = new AuthenticationService();
        }

        public async Task<APIGatewayProxyResponse> RefreshToken(APIGatewayProxyRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Body))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Request body is missing" });
                }

                var requestBody = JsonSerializer.Deserialize<RefreshTokenRequest>(request.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (requestBody == null || string.IsNullOrEmpty(requestBody.RefreshToken))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Refresh token is missing" });
                }

                var tokenResponse = await _authenticationService.RefreshToken(requestBody.RefreshToken);

                return ActionUtils.FormatResponse(200, tokenResponse);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine($"AuthenticationException: {ex.Message}");
                return ActionUtils.FormatResponse(400, new { message = $"Token refresh failed: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return ActionUtils.FormatResponse(500, new { message = "An unexpected error occurred" });
            }
        }
    }
}
