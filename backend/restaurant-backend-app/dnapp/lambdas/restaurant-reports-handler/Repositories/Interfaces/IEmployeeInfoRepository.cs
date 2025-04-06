using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IEmployeeInfoRepository
    {
        public Task<string> GetEmployeeEmail(string employeeId);
    }
}
