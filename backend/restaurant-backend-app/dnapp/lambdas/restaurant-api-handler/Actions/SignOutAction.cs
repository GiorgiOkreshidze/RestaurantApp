using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using SimpleLambdaFunction.Actions;

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
                if (!request.Headers.TryGetValue("Authorization", out var authHeader) || string.IsNullOrEmpty(authHeader))
                {
                    return ActionUtils.FormatResponse(401, new { message = "Authorization header is missing" });
                }
                
                var accessToken = authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : authHeader;

                if (string.IsNullOrEmpty(accessToken))
                {
                    return ActionUtils.FormatResponse(401, new { message = "Access token is invalid or missing" });
                }

                await _authenticationService.SignOut(accessToken);

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
