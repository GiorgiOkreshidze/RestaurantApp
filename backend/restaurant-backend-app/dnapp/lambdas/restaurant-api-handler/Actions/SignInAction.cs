using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using System.Security.Authentication;

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
        try
        {
            var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body ?? "{}",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var requiredParams = new[] { "email", "password" };
            var missingParams = ActionUtils.ValidateRequestParams(requiredParams, body);

            if (missingParams.Count > 0)
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = $"Missing required parameters: {string.Join(", ", missingParams)}"
                });
            }

            var email = body["email"].GetString();
            var password = body["password"].GetString();

            // Validate email and password
            if (string.IsNullOrEmpty(email) || !ActionUtils.ValidateEmail(email))
            {
                return ActionUtils.FormatResponse(400, new { message = "Invalid email format" });
            }

            if (string.IsNullOrEmpty(password))
            {
                return ActionUtils.FormatResponse(400, new { message = "Password cannot be empty" });
            }

            try
            {
                var authResponse = await _authenticationService.SignIn(email, password);
                return ActionUtils.FormatResponse(200, new
                {
                    accessToken = authResponse.AuthenticationResult.AccessToken,
                    idToken = authResponse.AuthenticationResult.IdToken,
                });
            }
            catch (AuthenticationException ex)
            {
                return ActionUtils.FormatResponse(401, new { message = ex.Message });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ActionUtils.FormatResponse(400, new
            {
                message = "Login failed. Please check your credentials and try again."
            });
        }
    }
}