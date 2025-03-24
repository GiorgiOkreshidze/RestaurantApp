using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

class GetProfileAction
{
    private readonly IAuthenticationService _authenticationService;

    public GetProfileAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> GetProfile(APIGatewayProxyRequest request)
    {
        if (!request.Headers.TryGetValue("x-amz-security-token", out var accessTokenHeader) ||
            string.IsNullOrEmpty(accessTokenHeader))
        {
            throw new UnauthorizedException("Unauthorized: No access token found.");
        }

        var accessToken = accessTokenHeader.Trim();
        var userDetails = await _authenticationService.GetUserDetailsAsync(accessToken);
        var firstName = userDetails.GetValueOrDefault("given_name");
        var lastName = userDetails.GetValueOrDefault("family_name");
        var email = userDetails.GetValueOrDefault("email");
        var role = userDetails.GetValueOrDefault("custom:role");
        var profileInfo = new ProfileInfo
        {
            FirstName = firstName!,
            LastName = lastName!,
            Email = email!,
            Role = role!,
            ImageUrl = "Here should be s3 img url"
        };
        
        return ActionUtils.FormatResponse(200, profileInfo);
    }
}