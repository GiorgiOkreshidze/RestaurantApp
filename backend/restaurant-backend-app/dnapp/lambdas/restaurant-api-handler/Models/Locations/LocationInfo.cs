using System.Text.Json.Serialization;

namespace Function.Models
{
    public class LocationInfo
    {
        [JsonPropertyName("locationId")]
        public required string LocationId { get; set; }

        [JsonPropertyName("address")]
        public required string Address { get; set; }

        [JsonPropertyName("totalCapacity")]
        public required string TotalCapacity { get; set; }

        [JsonPropertyName("averageOccupancy")]
        public required string AverageOccupancy { get; set; }
    }
}
