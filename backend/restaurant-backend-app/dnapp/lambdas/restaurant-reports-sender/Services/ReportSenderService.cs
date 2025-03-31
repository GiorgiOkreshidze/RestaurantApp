using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Function.Repositories.Interfaces;
using Function.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Services
{
    public class ReportSenderService : IReportSenderService
    {
        private readonly IReportRepository _reportRepository;
        private readonly AmazonSimpleEmailServiceClient _sesClient;

        public ReportSenderService(IReportRepository reportRepository, AmazonSimpleEmailServiceClient sesClient)
        {
            _reportRepository = reportRepository;
            _sesClient = sesClient;
        }

        public Task SendReportAsync(Dictionary<string, object> eventData)
        {
            var currentWeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var currentWeekEnd = currentWeekStart.AddDays(6);
            var previousWeekStart = currentWeekStart.AddDays(-7);
            var previousWeekEnd = currentWeekEnd.AddDays(-7);

            var currentWeekReports = _reportRepository.RetrieveReports(currentWeekStart, currentWeekEnd);
            var previousWeekReports = _reportRepository.RetrieveReports(previousWeekStart, previousWeekEnd);



            throw new System.NotImplementedException();
        }
    }
}
