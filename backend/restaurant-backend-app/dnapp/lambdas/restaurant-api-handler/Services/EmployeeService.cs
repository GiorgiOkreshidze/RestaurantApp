using System.Threading.Tasks;
using Function.Models.User;
using Function.Repository;
using Function.Repository.Interfaces;
using Function.Services.Interfaces;
using SimpleLambdaFunction.Repository;

namespace Function.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    
    public EmployeeService()
    {
        _employeeRepository = new EmployeeRepository();
    }
    
    public async Task<EmployeeInfo?> GetEmployeeInfoByEmailAsync(string email)
    {
        return await _employeeRepository.GetEmployeeInfoByEmailAsync(email);
    }
}