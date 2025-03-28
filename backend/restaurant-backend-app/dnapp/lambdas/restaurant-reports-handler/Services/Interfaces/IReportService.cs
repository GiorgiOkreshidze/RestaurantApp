using Function.Models;
using System.Threading.Tasks;

namespace Function.Services.Interfaces
{
    public interface IReportService
    {
        public Task ProcessReservationAsync(Reservation reservation);
    }
}
