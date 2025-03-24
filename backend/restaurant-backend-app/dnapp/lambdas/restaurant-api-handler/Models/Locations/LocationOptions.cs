using System.Text.Json.Serialization;

namespace Function.Models
{
    public class LocationOptions
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("address")]
        public required string Address { get; set; }
    }
}
