using Function.Models;
using Function.Repositories.Interfaces;
using Function.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Function.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFeedbacksRepository _feedbacksRepository;

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeInfoRepository employeeInfoRepository,
            IUserRepository userRepository,
            IFeedbacksRepository feedbacksRepository)
        {
            _reportRepository = reportRepository;
            _employeeInfoRepository = employeeInfoRepository;
            _userRepository = userRepository;
            _feedbacksRepository = feedbacksRepository;
        }

        public async Task ProcessReservationAsync(Reservation reservation)
        {
            var timeFrom = TimeSpan.Parse(reservation.TimeFrom);
            var timeTo = TimeSpan.Parse(reservation.TimeTo);
            var hoursWorked = (int)(timeTo - timeFrom).TotalHours;

            var email = await _userRepository.GetUserEmail(reservation.WaiterId!);

            var report = new Report
            {
                Id = Guid.NewGuid().ToString(),
                Date = reservation.Date,
                Location = reservation.LocationAddress,
                Waiter = await _userRepository.GetUserFullName(email),
                WaiterEmail = email,
                HoursWorked = hoursWorked,
                AverageServiceFeedback = await GetAverageServiceFeedback(reservation.Id),
                MinimumServiceFeedback = await GetMinimumServiceFeedback(reservation.Id)
            };

            await _reportRepository.SaveReportAsync(report);
        }

        private async Task<int> GetMinimumServiceFeedback(string id)
        {
            var feedbacks = await _feedbacksRepository.GetServiceFeedbacks(id);

            if (feedbacks == null || feedbacks.Count == 0)
            {
                return 0;
            }

            return feedbacks.Min(f => f.Rate);
        }

        private async Task<double> GetAverageServiceFeedback(string id)
        {
            var feedbacks = await _feedbacksRepository.GetServiceFeedbacks(id);
            if (feedbacks == null || feedbacks.Count == 0)
            {
                return 0;
            }

            return feedbacks.Average(f => f.Rate);
        }
    }
}
