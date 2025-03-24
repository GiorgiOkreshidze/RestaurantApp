using System.Threading.Tasks;
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
    
    public async Task<bool> CheckIfEmailExistsInWaitersTableAsync(string email)
    {
        return await _employeeRepository.CheckIfEmailExistsInWaitersTableAsync(email);
    }
}