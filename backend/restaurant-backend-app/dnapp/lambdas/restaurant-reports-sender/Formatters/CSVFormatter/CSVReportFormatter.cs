using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Formatters.CSVFormatter
{
    public class CsvReportFormatter : IReportFormatter
    {
        public string Format(List<SummaryEntry> summary)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Location,Start Date,End Date,Waiter Name,Waiter Email,Current Hours,Next Hours,Delta Hours");
            foreach (var entry in summary)
            {
                csvBuilder.AppendLine($"{entry.Location},{entry.StartDate},{entry.EndDate},{entry.WaiterName},{entry.WaiterEmail},{entry.CurrentHours},{entry.NextHours},{entry.DeltaHours}");
            }
            return csvBuilder.ToString();
        }
    }
}
