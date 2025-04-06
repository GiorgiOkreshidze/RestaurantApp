using System.Text.Json.Serialization;

namespace Function.Models
{
    public class ReservationInfo
    {
        [JsonPropertyName("tableId")]
        public required string TableId { get; set; }

        [JsonPropertyName("tableNumber")]
        public required string TableNumber { get; set; }

        [JsonPropertyName("date")]
        public required string Date { get; set; }

        [JsonPropertyName("timeFrom")]
        public required string TimeFrom { get; set; }

        [JsonPropertyName("timeTo")]
        public required string TimeTo { get; set; }

        [JsonPropertyName("guestNumber")]
        public required string GuestsNumber { get; set; }
    }
}
