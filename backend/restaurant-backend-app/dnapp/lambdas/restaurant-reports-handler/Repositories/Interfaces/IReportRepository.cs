using Function.Models;
using System.Threading.Tasks;

namespace Function.Repositories.Interfaces
{
    public interface IReportRepository
    {
        public Task SaveReportAsync(Report report);
    }
}
