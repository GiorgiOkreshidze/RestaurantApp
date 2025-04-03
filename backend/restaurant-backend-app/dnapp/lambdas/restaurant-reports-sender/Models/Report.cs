using System.Text.Json.Serialization;

namespace Function.Models
{
    public class Report
    {
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
