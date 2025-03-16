using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using SimpleLambdaFunction.Actions;
using System.Text.Json;
using Function.Models;

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
                if (string.IsNullOrEmpty(request.Body))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Request body is missing" });
                }

                var requestBody = JsonSerializer.Deserialize<SignOutRequest>(request.Body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (requestBody == null || string.IsNullOrEmpty(requestBody.RefreshToken))
                {
                    return ActionUtils.FormatResponse(400, new { message = "Refresh token is missing" });
                }

                await _authenticationService.SignOut(requestBody.RefreshToken);

                return ActionUtils.FormatResponse(200, new { message = "Successfully signed out" });
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine(ex);
                return ActionUtils.FormatResponse(400, new { message = $"Sign-out failed: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ActionUtils.FormatResponse(400, new { message = $"Sign-out failed: {ex.Message}" });
            }
        }
    }
}
