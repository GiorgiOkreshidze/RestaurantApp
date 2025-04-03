using Function.Models;
using Function.Repositories.Interfaces;
using Function.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Function.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IUserRepository _userRepository;

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeInfoRepository employeeInfoRepository,
            IUserRepository userRepository)
        {
            _reportRepository = reportRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _userRepository = userRepository;
        }

        public async Task ProcessReservationAsync(Reservation reservation)
        {
            var timeFrom = TimeSpan.Parse(reservation.TimeFrom);
            var timeTo = TimeSpan.Parse(reservation.TimeTo);
            var hoursWorked = (int)(timeTo - timeFrom).TotalHours;

            var email = await _employeeInfoRepository.GetEmployeeEmail(reservation.WaiterId!);

            var report = new Report
            {
                Id = Guid.NewGuid().ToString(),
                Date = reservation.Date,
                Location = reservation.LocationAddress,
                Waiter = await _userRepository.GetUserFullName(email),
                WaiterEmail = email,
                HoursWorked = hoursWorked
            };

            await _reportRepository.SaveReportAsync(report);
        }
    }
}
