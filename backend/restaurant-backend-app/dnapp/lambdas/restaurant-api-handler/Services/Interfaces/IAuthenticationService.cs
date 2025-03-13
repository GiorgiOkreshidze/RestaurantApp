using Function.Models;
using System.Threading.Tasks;

namespace SimpleLambdaFunction.Services.Interfaces;

public interface IAuthenticationService
{
    Task<string> SignIn(string email, string password);
    Task SignUp(string firstName, string lastName, string email, string password, Roles role = Roles.Customer);
    Task SignOut(string accessToken);
    Task CheckEmailUniqueness(string email);
}