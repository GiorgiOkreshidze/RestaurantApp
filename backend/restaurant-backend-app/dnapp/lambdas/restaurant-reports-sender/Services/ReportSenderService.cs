using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Function.Formatters;
using Function.Formatters.ExcelFormatter;
using Function.Mappers;
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
        private readonly IReportFormatter _reportFormatter;

        private const string FromEmail = "wojomi5508@macho3.com";
        private const string ToEmail = "kamaro5226@provko.com";

        public ReportSenderService(
            IReportRepository reportRepository,
            AmazonSimpleEmailServiceClient sesClient,
            IReportFormatter reportFormatter
            )
        {
            _reportRepository = reportRepository;
            _sesClient = sesClient;
            _reportFormatter = reportFormatter;
        }

        public async Task SendReportAsync(Dictionary<string, object> eventData)
        {
            var currentWeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var currentWeekEnd = currentWeekStart.AddDays(6);
            var previousWeekStart = currentWeekStart.AddDays(-7);
            var previousWeekEnd = currentWeekEnd.AddDays(-7);

            var currentWeekItems = await _reportRepository.RetrieveReports(currentWeekStart, currentWeekEnd);
            var previousWeekItems = await _reportRepository.RetrieveReports(previousWeekStart, previousWeekEnd);

            var currentWeekReports = Mapper.MapItemsToReports(currentWeekItems);
            var previousWeekReports = Mapper.MapItemsToReports(previousWeekItems);

            var summary = ProcessReports(currentWeekReports, previousWeekReports, currentWeekStart, currentWeekEnd);

            var reportContent = _reportFormatter.Format(summary);

            string fileName = _reportFormatter is ExcelReportFormatter ? "report.xlsx" : "report.csv";
            string mimeType = _reportFormatter is ExcelReportFormatter ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

            await SendEmailAsync(reportContent, fileName, mimeType);
        }

        private List<SummaryEntry> ProcessReports(List<Report> currentWeek, List<Report> previousWeek, DateTime startDate, DateTime endDate)
        {
            var waiterSummaries = new Dictionary<string, SummaryEntry>();
            var currentFeedbackCount = new Dictionary<string, int>();
            var previousFeedbackCount = new Dictionary<string, int>();

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
                        PreviousHours = 0,
                        DeltaHours = 0,
                        CurrentAverageServiceFeedback = 0,
                        MinimumServiceFeedback = int.MaxValue,
                        PreviousAverageServiceFeedback = 0,
                        DeltaAverageServiceFeedback = 0
                    };

                    currentFeedbackCount[key] = 0;
                    previousFeedbackCount[key] = 0;
                }

                waiterSummaries[key].CurrentHours += report.HoursWorked;
                waiterSummaries[key].CurrentAverageServiceFeedback += report.AverageServiceFeedback;
                currentFeedbackCount[key]++;

                waiterSummaries[key].MinimumServiceFeedback = Math.Min(waiterSummaries[key].MinimumServiceFeedback, report.MinimumServiceFeedback);
            }

            // Aggregate previous week
            foreach (var report in previousWeek)
            {
                var key = $"{ report.Waiter}-{ report.WaiterEmail}";
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
                        PreviousHours = 0,
                        DeltaHours = 0,
                        CurrentAverageServiceFeedback = 0,
                        MinimumServiceFeedback = int.MaxValue,
                        PreviousAverageServiceFeedback = 0,
                        DeltaAverageServiceFeedback = 0
                    };

                    currentFeedbackCount[key] = 0;
                    previousFeedbackCount[key] = 0;
                }

                waiterSummaries[key].PreviousHours += report.HoursWorked;
                waiterSummaries[key].PreviousAverageServiceFeedback += report.AverageServiceFeedback;
                previousFeedbackCount[key]++;
            }

            foreach (var key in waiterSummaries.Keys)
            {
                var entry = waiterSummaries[key];

                // Compute delta hours as percentage difference (using 1 for +100% when no previous hours)
                if (entry.PreviousHours == 0)
                {
                    entry.DeltaHours = entry.CurrentHours > 0 ? 1 : 0;
                }
                else
                {
                    entry.DeltaHours = ((entry.CurrentHours - entry.PreviousHours) / entry.PreviousHours);
                }

                // Calculate average service feedback for current week
                if (currentFeedbackCount[key] > 0)
                {
                    entry.CurrentAverageServiceFeedback /= currentFeedbackCount[key];
                }
                // Calculate average service feedback for previous week
                if (previousFeedbackCount[key] > 0)
                {
                    entry.PreviousAverageServiceFeedback /= previousFeedbackCount[key];
                }

                if (entry.PreviousAverageServiceFeedback == 0)
                {
                    entry.DeltaAverageServiceFeedback = entry.CurrentAverageServiceFeedback > 0 ? 1 : 0;
                }
                else
                {
                    entry.DeltaAverageServiceFeedback = (entry.CurrentAverageServiceFeedback - entry.PreviousAverageServiceFeedback) / entry.PreviousAverageServiceFeedback;
                }

                // If no feedback was recorded in the current week, set minimum to 0 instead of int.MaxValue
                if (entry.MinimumServiceFeedback == int.MaxValue)
                {
                    entry.MinimumServiceFeedback = 0;
                }
            }

            return waiterSummaries.Values.ToList();
        }


        private async Task SendEmailAsync(string reportContent, string fileName, string mimeType)
        {
            try
            {
                MemoryStream attachmentStream;
                if (mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    // For Excel, reportContent is base64; decode it to bytes
                    attachmentStream = new MemoryStream(Convert.FromBase64String(reportContent));
                }
                else
                {
                    // For CSV, reportContent is a UTF-8 string
                    attachmentStream = new MemoryStream(Encoding.UTF8.GetBytes(reportContent));
                }

                using (attachmentStream)
                {
                    var request = new SendRawEmailRequest
                    {
                        RawMessage = new RawMessage
                        {
                            Data = CreateEmailMessage(attachmentStream, fileName, mimeType)
                        }
                    };
                    await _sesClient.SendRawEmailAsync(request);
                }
            }
            catch (Exception ex)
            {
                throw; // Re-throw to let Lambda handle retries or logging
            }
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
