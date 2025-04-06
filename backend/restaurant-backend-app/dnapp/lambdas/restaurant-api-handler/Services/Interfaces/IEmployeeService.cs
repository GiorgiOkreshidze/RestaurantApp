using System.Threading.Tasks;
using Function.Models.User;

namespace Function.Services.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeInfo?> GetEmployeeInfoByEmailAsync(string email);
}