using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class TableResponse
    {
        [JsonPropertyName("availableSlots")]
        public required List<TimeSlot> AvailableSlots { get; set; }

        [JsonPropertyName("capacity")]
        public required string Capacity { get; set; }

        [JsonPropertyName("locationAddress")]
        public required string LocationAddress { get; set; }

        [JsonPropertyName("locationId")]
        public required string LocationId { get; set; }

        [JsonPropertyName("tableNumber")]
        public required string TableNumber { get; set; }
    }
}
