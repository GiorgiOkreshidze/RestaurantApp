using Amazon.DynamoDBv2.Model;
using Function.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Mappers
{
    internal class Mapper
    {
        public static List<Report> MapItemsToReports(List<Dictionary<string, AttributeValue>> items)
        {
            return items.Select(doc => new Report
            {
                Id = doc.TryGetValue("id", out var id) ? id.S : string.Empty,
                Location = doc.TryGetValue("location", out var location) ? location.S : string.Empty,
                Date = doc.TryGetValue("date", out var date) ? date.S : string.Empty,
                Waiter = doc.TryGetValue("waiter", out var waiter) ? waiter.S : string.Empty,
                WaiterEmail = doc.TryGetValue("waiterEmail", out var waiterEmail) ? waiterEmail.S : string.Empty,
                HoursWorked = doc.TryGetValue("hoursWorked", out var hoursWorked) ? int.Parse(hoursWorked.N) : 0,
                AverageServiceFeedback = doc.TryGetValue("averageServiceFeedback", out var averageServiceFeedback) 
                ? double.Parse(averageServiceFeedback.N) 
                : 0.0,
                MinimumServiceFeedback = doc.TryGetValue("minimumServiceFeedback", out var minimumServiceFeedback) 
                ? int.Parse(minimumServiceFeedback.N) 
                : 0
            }).ToList();
        }
    }
}
