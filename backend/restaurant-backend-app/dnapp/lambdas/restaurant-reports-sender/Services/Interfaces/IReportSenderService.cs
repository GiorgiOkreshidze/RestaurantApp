
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Services.Interfaces
{
    public interface IReportSenderService
    {
        public Task SendReportAsync(Dictionary<string, object> eventData);
    }
}
