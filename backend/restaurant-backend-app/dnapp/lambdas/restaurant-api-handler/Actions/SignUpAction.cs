using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;

namespace SimpleLambdaFunction.Actions;

public class SignUpAction
{
    private readonly IAuthenticationService _authenticationService;

    public SignUpAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> Signup(APIGatewayProxyRequest request)
    {
        await _authenticationService.SignUp("name", "last name", "test@test.com", "123456");
        return new APIGatewayProxyResponse();
    }
}