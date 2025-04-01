using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Users;

public class GetAllCustomersAction
{
    private readonly IUserService _userService;

    public GetAllCustomersAction()
    {
        _userService = new UserService();
    }

    public async Task<APIGatewayProxyResponse> GetAllCustomersAsync(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);

        if (!Enum.TryParse(jwtToken.Claims.FirstOrDefault(c => c.Type == "custom:role")?.Value, out Roles role))
        {
            throw new ArgumentException("Invalid role");
        }        
        
        if (role != Roles.Waiter)
        {
            throw new UnauthorizedException("Unauthorized: You don't have permission to access this resource.");
        }
        
        var users = await _userService.GetAllCustomersAsync();
        
        return ActionUtils.FormatResponse(200, users);
    }
}