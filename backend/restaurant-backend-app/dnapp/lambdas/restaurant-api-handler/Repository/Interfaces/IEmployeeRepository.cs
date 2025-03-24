using System.Threading.Tasks;

namespace Function.Repository.Interfaces;

public interface IEmployeeRepository
{
    Task<bool> CheckIfEmailExistsInWaitersTableAsync(string email);
}