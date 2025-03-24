using System.Threading.Tasks;

namespace Function.Services.Interfaces;

public interface IEmployeeService
{
    Task<bool> CheckIfEmailExistsInWaitersTableAsync(string email);
}