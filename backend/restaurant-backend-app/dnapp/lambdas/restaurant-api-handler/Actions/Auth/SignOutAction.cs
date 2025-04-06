using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

class SignOutAction
{
    private readonly IAuthenticationService _authenticationService;

    public SignOutAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> SignOut(APIGatewayProxyRequest request)
    {
        ActionUtils.ValidateRequestBody(request.Body);

        var signOutRequest = JsonSerializer.Deserialize<SignOutRequest>(request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (signOutRequest == null || string.IsNullOrEmpty(signOutRequest.RefreshToken))
        {
            throw new ArgumentException("Refresh token is missing");
        }

        await _authenticationService.SignOut(signOutRequest.RefreshToken);
        return ActionUtils.FormatResponse(200, new { message = "Successfully signed out" });
    }
}