using System.Threading.Tasks;

namespace SimpleLambdaFunction.Services.Interfaces;

public interface IAuthenticationService
{
    public Task<string> SignIn(string email, string password);
    public Task SignUp(string firstName, string lastName, string email, string password);
}