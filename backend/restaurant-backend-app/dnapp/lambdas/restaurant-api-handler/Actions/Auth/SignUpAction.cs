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
    private readonly IUserService _userService;
    private const string DefaultImageUrl = "https://team2-demo-bucket.s3.eu-west-2.amazonaws.com/Images/Users/default_user.jpg";

    public SignUpAction()
    {
        _authenticationService = new AuthenticationService();
        _employeeService = new EmployeeService();
        _userService = new UserService();
    }

    public async Task<APIGatewayProxyResponse> Signup(APIGatewayProxyRequest request)
    {
        var body = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(request.Body ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        if (body == null) throw new ArgumentException("Request body is empty");

        var requiredParams = new[] { "firstName", "lastName", "email", "password" };
        
        ActionUtils.ValidateRequiredParams(requiredParams, body);

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = body["firstName"].GetString(),
            LastName = body["lastName"].GetString(),
            Email = body["email"].GetString(),
            Role = Roles.Customer,
            ImageUrl = DefaultImageUrl,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
        var password = body["password"].GetString();
        
        ActionUtils.ValidateFullName(user.FirstName);
        ActionUtils.ValidateFullName(user.LastName);
        ActionUtils.ValidateEmail(user.Email);
        ActionUtils.ValidatePassword(password);

        await _authenticationService.CheckEmailUniqueness(user.Email);

        var result = await SignUpUserWithRole(user, password);
        return ActionUtils.FormatResponse(200, new { message = result });
    }

    private async Task<string> SignUpUserWithRole(User user, string password)
    {

        var employeeInfo = await _employeeService.GetEmployeeInfoByEmailAsync(user.Email);

        if (employeeInfo == null)
        {
            Console.WriteLine("User with email not found in employeeInfo table, so its just regular customer");
            await _authenticationService.SignUp(user, password);
        }
        else
        {
            await _authenticationService.SignUp(user, password, Roles.Waiter);
            user.LocationId = employeeInfo.LocationId;
            user.Role = Roles.Waiter;
        }
        
        await _userService.AddUserAsync(user);
        return "User Registered";
    }
}