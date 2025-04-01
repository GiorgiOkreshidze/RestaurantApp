using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Function.Formatters;
using Function.Formatters.CSVFormatter;
using Function.Models;
using Function.Repositories.Interfaces;
using Function.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Services
{
    public class ReportSenderService : IReportSenderService
    {
        private readonly IReportRepository _reportRepository;
        private readonly AmazonSimpleEmailServiceClient _sesClient;

        private const string FromEmail = "sender@example.com";
        private const string ToEmail = "recipient@example.com";

        public ReportSenderService(IReportRepository reportRepository, AmazonSimpleEmailServiceClient sesClient)
        {
            _reportRepository = reportRepository;
            _sesClient = sesClient;
        }

        public async Task SendReportAsync(Dictionary<string, object> eventData)
        {
            var currentWeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var currentWeekEnd = currentWeekStart.AddDays(6);
            var previousWeekStart = currentWeekStart.AddDays(-7);
            var previousWeekEnd = currentWeekEnd.AddDays(-7);

            var currentWeekReports = await _reportRepository.RetrieveReports(currentWeekStart, currentWeekEnd);
            var previousWeekReports = await _reportRepository.RetrieveReports(previousWeekStart, previousWeekEnd);

            var summary = ProcessReports(currentWeekReports, previousWeekReports, currentWeekStart, currentWeekEnd);

            IReportFormatter formatter = new CsvReportFormatter();
            var reportContent = formatter.Format(summary);

            // Send email
            await SendEmailAsync(reportContent, "report.csv", "text/csv");
        }

        private List<SummaryEntry> ProcessReports(List<Report> currentWeek, List<Report> nextWeek, DateTime startDate, DateTime endDate)
        {
            var waiterSummaries = new Dictionary<string, SummaryEntry>();

            // Aggregate current week
            foreach (var report in currentWeek)
            {
                var key = $"{report.Waiter}-{report.WaiterEmail}";
                if (!waiterSummaries.ContainsKey(key))
                {
                    waiterSummaries[key] = new SummaryEntry
                    {
                        Location = report.Location,
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd"),
                        WaiterName = report.Waiter,
                        WaiterEmail = report.WaiterEmail,
                        CurrentHours = 0,
                        NextHours = 0
                    };
                }
                waiterSummaries[key].CurrentHours += report.HoursWorked;
            }

            // Aggregate next week
            foreach (var report in nextWeek)
            {
                var key = $"{report.Waiter}-{report.WaiterEmail}";
                if (!waiterSummaries.ContainsKey(key))
                {
                    waiterSummaries[key] = new SummaryEntry
                    {
                        Location = report.Location,
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd"),
                        WaiterName = report.Waiter,
                        WaiterEmail = report.WaiterEmail,
                        CurrentHours = 0,
                        NextHours = 0
                    };
                }
                waiterSummaries[key].NextHours += report.HoursWorked;
            }

            // Calculate delta
            foreach (var entry in waiterSummaries.Values)
            {
                entry.DeltaHours = entry.NextHours - entry.CurrentHours;
            }

            return waiterSummaries.Values.ToList();
        }


        private async Task SendEmailAsync(string reportContent, string fileName, string mimeType)
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(reportContent));
            var request = new SendRawEmailRequest
            {
                RawMessage = new RawMessage
                {
                    Data = CreateEmailMessage(memoryStream, fileName, mimeType)
                }
            };
            await _sesClient.SendRawEmailAsync(request);
        }

        private MemoryStream CreateEmailMessage(MemoryStream attachment, string fileName, string mimeType)
        {
            var boundary = "----=_Part_" + Guid.NewGuid().ToString();
            var emailBuilder = new StringBuilder();
            emailBuilder.AppendLine($"From: {FromEmail}");
            emailBuilder.AppendLine($"To: {ToEmail}");
            emailBuilder.AppendLine("Subject: Weekly Report Summary");
            emailBuilder.AppendLine($"MIME-Version: 1.0");
            emailBuilder.AppendLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine($"--{boundary}");
            emailBuilder.AppendLine("Content-Type: text/plain; charset=UTF-8");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine("Please find the attached weekly report.");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine($"--{boundary}");
            emailBuilder.AppendLine($"Content-Type: {mimeType}; name=\"{fileName}\"");
            emailBuilder.AppendLine("Content-Disposition: attachment; filename=\"" + fileName + "\"");
            emailBuilder.AppendLine("Content-Transfer-Encoding: base64");
            emailBuilder.AppendLine();
            emailBuilder.AppendLine(Convert.ToBase64String(attachment.ToArray(), Base64FormattingOptions.InsertLineBreaks));
            emailBuilder.AppendLine();
            emailBuilder.AppendLine($"--{boundary}--");

            return new MemoryStream(Encoding.UTF8.GetBytes(emailBuilder.ToString()));
        }
    }
}
