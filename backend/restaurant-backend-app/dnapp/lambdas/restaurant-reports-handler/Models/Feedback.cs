using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Function.Models
{
    public class Feedback
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("rate")]
        public required int Rate { get; set; }

        [JsonPropertyName("comment")]
        public required string Comment { get; set; }

        [JsonPropertyName("userName")]
        public required string UserName { get; set; }

        [JsonPropertyName("userAvatarUrl")]
        public required string UserAvatarUrl { get; set; }

        [JsonPropertyName("date")]
        public required string Date { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; }

        [JsonPropertyName("locationId")]
        public required string LocationId { get; set; }

        [JsonPropertyName("reservationId")]
        public required string ReservationId { get; set; }
    }
}
