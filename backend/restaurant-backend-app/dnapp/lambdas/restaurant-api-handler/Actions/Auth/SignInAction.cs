using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

public class SignInAction
{
    private readonly IAuthenticationService _authenticationService;

    public SignInAction()
    {
        _authenticationService = new AuthenticationService();
    }

    public async Task<APIGatewayProxyResponse> Signin(APIGatewayProxyRequest request)
    {
        var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (body == null) throw new ArgumentException("Request body is empty");
        
        var requiredParams = new[] { "email", "password" };

        ActionUtils.ValidateRequiredParams(requiredParams, body);

        var email = body["email"].GetString();
        var password = body["password"].GetString();

        ActionUtils.ValidateEmail(email);
        ActionUtils.ValidatePassword(password);
        
        var authResponse = await _authenticationService.SignIn(email, password);
        return ActionUtils.FormatResponse(200, authResponse);
    }
}