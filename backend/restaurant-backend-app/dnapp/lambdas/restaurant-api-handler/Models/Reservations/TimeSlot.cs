using System.Text.Json.Serialization;

namespace Function.Models.Reservations;

public class TimeSlot
{
    [JsonPropertyName("start")]
    public required string Start { get; set; }

    [JsonPropertyName("end")]
    public required string End { get; set; }
}