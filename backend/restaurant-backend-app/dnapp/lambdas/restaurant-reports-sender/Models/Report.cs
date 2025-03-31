using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    [DynamoDBTable("Reports")]
    public class Report
    {
        [DynamoDBHashKey]
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("location")]
        public required string Location { get; set; }
        [JsonPropertyName("date")]
        public required string Date { get; set; }
        [JsonPropertyName("waiter")]
        public required string Waiter { get; set; }
        [JsonPropertyName("waiterEmail")]
        public required string WaiterEmail { get; set; }
        [JsonPropertyName("hoursWorked")]
        public required int HoursWorked { get; set; }
    }
}
