using System.Collections.Generic;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using SimpleLambdaFunction.Services;
using SimpleLambdaFunction.Services.Interfaces;
using Function.Exceptions;
using Function.Services.Interfaces;
using Function.Services;
using Function.Models;

namespace SimpleLambdaFunction.Actions;

public class SignUpAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IDynamoDBService _dynamoDBService;
    public SignUpAction()
    {
        _authenticationService = new AuthenticationService();
        _dynamoDBService = new DynamoDBService();
    }

    public async Task<APIGatewayProxyResponse> Signup(APIGatewayProxyRequest request)
    {
        try
        {
            var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body ?? "{}",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var requiredParams = new[] { "firstName", "lastName", "email", "password" };
            var missingParams = ActionUtils.ValidateRequestParams(requiredParams, body);

            if (missingParams.Count > 0)
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = $"Missing required parameters: {string.Join(", ", missingParams)}"
                });
            }

            var firstName = body["firstName"].GetString();
            var lastName = body["lastName"].GetString();
            var email = body["email"].GetString();
            var password = body["password"].GetString();


            if (!ActionUtils.ValidateName(firstName))
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = "First name must be up to 50 characters. Only Latin letters, hyphens, and apostrophes are allowed."
                });
            }

            if (!ActionUtils.ValidateName(lastName))
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = "Last name must be up to 50 characters. Only Latin letters, hyphens, and apostrophes are allowed."
                });
            }

            if (!ActionUtils.ValidateEmail(email))
            {
                return ActionUtils.FormatResponse(400, new { message = "Invalid email format. Please ensure it follows the format: username@domain.com" });
            }

            if (!ActionUtils.ValidatePassword(password))
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = "Password must be 8-16 characters long and include at least one uppercase letter, one lowercase letter, one number, and one special character."
                });
            }

            try
            {
                await _authenticationService.CheckEmailUniqueness(email);
            }
            catch (UserExistsException)
            {
                return ActionUtils.FormatResponse(400, new
                {
                    message = "Email is already registered. Please use a different email address."
                });
            }

            var result = await SignUpUserWithRole(firstName, lastName, email, password);

            return ActionUtils.FormatResponse(200, result);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ActionUtils.FormatResponse(400, new { message = $"Signup failed: {ex.Message}" });
        }
    }

    private async Task<string> SignUpUserWithRole(string firstName, string lastName, string email, string password)
    {
        var isWaiter = await _dynamoDBService.CheckIfEmailExistsInWaitersTable(email);

        if (isWaiter)
        {
            await _authenticationService.SignUp(firstName, lastName, email, password, Roles.Waiter);
        }
        else
        {
            await _authenticationService.SignUp(firstName, lastName, email, password);
        }

        return "User Registered";
    }
}