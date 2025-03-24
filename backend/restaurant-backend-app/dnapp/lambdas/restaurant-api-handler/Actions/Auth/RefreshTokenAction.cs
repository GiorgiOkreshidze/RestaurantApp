using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

public class RefreshTokenAction
{
    private readonly IAuthenticationService _authenticationService;

    public RefreshTokenAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> RefreshToken(APIGatewayProxyRequest request)
    {
        ActionUtils.ValidateRequestBody(request.Body);

        var refreshTokenRequest = JsonSerializer.Deserialize<RefreshTokenRequest>(request.Body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken))
        {
            throw new ArgumentException("Refresh token is missing");
        }

        var tokenResponse = await _authenticationService.RefreshToken(refreshTokenRequest.RefreshToken);
        return ActionUtils.FormatResponse(200, tokenResponse);
    }
}