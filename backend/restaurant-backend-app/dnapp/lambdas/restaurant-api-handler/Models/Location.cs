using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Location
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("address")]
        public required string Address { get; set; }
        [JsonPropertyName("description")]
        public required string Description { get; set; }
        [JsonPropertyName("totalCapacity")]
        public required string TotalCapacity { get; set; }
        [JsonPropertyName("averageOccupancy")]
        public required string AverageOccupancy { get; set; }
        [JsonPropertyName("imageUrl")]
        public required string ImageUrl { get; set; }
        [JsonPropertyName("rating")]
        public required string Rating { get; set; }
    }
}
