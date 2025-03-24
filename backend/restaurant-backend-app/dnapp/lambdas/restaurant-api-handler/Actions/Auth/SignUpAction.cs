using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

public class SignUpAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmployeeService _employeeService;

    public SignUpAction()
    {
        _authenticationService = new AuthenticationService();
        _employeeService = new EmployeeService();
    }

    public async Task<APIGatewayProxyResponse> Signup(APIGatewayProxyRequest request)
    {
        var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (body == null) throw new ArgumentException("Request body is empty");

        var requiredParams = new[] { "firstName", "lastName", "email", "password" };
        
        ActionUtils.ValidateRequiredParams(requiredParams, body);

        var firstName = body["firstName"].GetString();
        var lastName = body["lastName"].GetString();
        var email = body["email"].GetString();
        var password = body["password"].GetString();
        
        ActionUtils.ValidateFullName(firstName);
        ActionUtils.ValidateFullName(lastName);
        ActionUtils.ValidateEmail(email);
        ActionUtils.ValidatePassword(password);

        await _authenticationService.CheckEmailUniqueness(email);

        var result = await SignUpUserWithRole(firstName, lastName, email, password);
        return ActionUtils.FormatResponse(200, new { message = result });
    }

    private async Task<string> SignUpUserWithRole(string firstName, string lastName, string email, string password)
    {
        var isWaiter = await _employeeService.CheckIfEmailExistsInWaitersTableAsync(email);

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