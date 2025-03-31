using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Users;

class GetProfileAction
{
    private readonly IUserService _userService;

    public GetProfileAction()
    {
        _userService = new UserService();
    }

    public async Task<APIGatewayProxyResponse> GetProfile(APIGatewayProxyRequest request)
    {
        var jwtToken = ActionUtils.ExtractJwtToken(request);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return ActionUtils.FormatResponse(403, new { message = "Forbidden: Resource not found." });
        }

        var userProfile = await _userService.GetUserByIdAsync(userId);

        return ActionUtils.FormatResponse(200, userProfile);
    }
}