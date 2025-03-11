using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;

namespace SimpleLambdaFunction.Actions;

public class SignInAction
{
    private readonly IAuthenticationService _authenticationService;

    public SignInAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> Signin(APIGatewayProxyRequest request)
    {
        var result = await _authenticationService.SignIn("test@test.com", "123456");
        return new APIGatewayProxyResponse();
    }
}