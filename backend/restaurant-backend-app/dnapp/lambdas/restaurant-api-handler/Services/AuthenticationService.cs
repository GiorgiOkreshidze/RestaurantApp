using System.Threading.Tasks;
using SimpleLambdaFunction.Services.Interfaces;

namespace SimpleLambdaFunction.Services;

public class AuthenticationService : IAuthenticationService
{
    public Task<string> SignIn(string email, string password)
    {
        throw new System.NotImplementedException();
    }

    public Task SignUp(string firstName, string lastName, string email, string password)
    {
        throw new System.NotImplementedException();
    }
}