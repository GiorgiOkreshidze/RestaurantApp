using System.Text.Json.Serialization;

namespace Function.Models
{
    public class RestaurantTable
    {
        [JsonPropertyName("tableId")]
        public required string TableId { get; set; }

        [JsonPropertyName("tableNumber")]
        public required string TableNumber { get; set; }

        [JsonPropertyName("capacity")]
        public required string Capacity { get; set; }

        [JsonPropertyName("locationId")]
        public required string LocationId { get; set; }

        [JsonPropertyName("locationAddress")]
        public required string LocationAddress { get; set; }
    }
}
