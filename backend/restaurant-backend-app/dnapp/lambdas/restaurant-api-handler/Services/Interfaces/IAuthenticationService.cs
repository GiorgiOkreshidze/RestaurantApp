using Function.Models;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;
using System.Collections.Generic;

namespace SimpleLambdaFunction.Services.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResult> SignIn(string email, string password);
    Task<AuthResult> SignUp(string firstName, string lastName, string email, string password, Roles role = Roles.Customer);
    Task SignOut(string accessToken);
    Task CheckEmailUniqueness(string email);
    Task<AuthResult> RefreshToken(string refreshToken);
    Task<Dictionary<string, string>> GetUserDetailsAsync(string accessToken);
}