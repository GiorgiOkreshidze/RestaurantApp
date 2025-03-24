using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Users;

public class GetAllUsersAction
{
    private readonly IUserService _userService;

    public GetAllUsersAction()
    {
        _userService = new UserService();
    }

    public async Task<APIGatewayProxyResponse> GetAllUsersAsync(APIGatewayProxyRequest request)
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
        
        if (!Enum.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "custom:role")?.Value, out Roles role))
        {
            throw new ArgumentException("Invalid role");
        }        
        
        if (role != Roles.Waiter)
        {
            throw new UnauthorizedException("Unauthorized: You don't have permission to access this resource.");
        }
        
        
        var users = await _userService.GetAllUsersAsync();
        
        return ActionUtils.FormatResponse(200, users);
    }
}