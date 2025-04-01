using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Function.Models.Requests;
using Function.Models.User;
using Function.Services;
using Function.Services.Interfaces;

namespace Function.Actions.Auth;

public class SignUpAction
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmployeeService _employeeService;
    private readonly IUserService _userService;
    private const string DefaultUserImageUrl = "https://team2-demo-bucket.s3.eu-west-2.amazonaws.com/Images/Users/default_user.jpg";

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

        var firstName = body["firstName"].GetString();
        var lastName = body["lastName"].GetString();
        var email = body["email"].GetString();
        var password = body["password"].GetString();

        ActionUtils.ValidateFullName(firstName);
        ActionUtils.ValidateFullName(lastName);
        ActionUtils.ValidateEmail(email);
        ActionUtils.ValidatePassword(password);

        await _authenticationService.CheckEmailUniqueness(email!);

        var result = await SignUpUserWithRole(firstName!, lastName!, email!, password!);
        return ActionUtils.FormatResponse(200, new { message = result });
    }

    private async Task<string> SignUpUserWithRole(string firstName, string lastName, string email, string password)
    {

        var role = Roles.Customer;
        string? locationId = null;
        var employeeInfo = await _employeeService.GetEmployeeInfoByEmailAsync(email);

        var cognitoUser = new CreateUserCognitoRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
        };

        string userId = employeeInfo == null
            ? await _authenticationService.SignUp(cognitoUser, password)
            : await _authenticationService.SignUp(cognitoUser, password, Roles.Waiter);

        if (employeeInfo != null)
        {
            role = Roles.Waiter;
            locationId = employeeInfo.LocationId;
        }

        var dynamoDbUser = new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            LocationId = locationId,
            ImageUrl = DefaultUserImageUrl,
            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };

        await _userService.AddUserAsync(dynamoDbUser);
        return "User Registered";
    }
}