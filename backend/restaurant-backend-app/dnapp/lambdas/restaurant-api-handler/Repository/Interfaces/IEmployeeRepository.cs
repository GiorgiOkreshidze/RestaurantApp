using System.Threading.Tasks;
using Function.Models.User;

namespace Function.Repository.Interfaces;

public interface IEmployeeRepository
{
    Task<EmployeeInfo?> GetEmployeeInfoByEmailAsync(string email);
}