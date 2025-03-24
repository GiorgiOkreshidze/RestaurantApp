using System.Text.Json.Serialization;

namespace Function.Models.Requests;

public class ReservationRequest
{
    // if no id, we are generating it otherwise, we update record based on the provided id.
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("locationId")]
    public required string LocationId { get; set; }
    
    [JsonPropertyName("tableId")]
    public required string TableId { get; set; }
 
    [JsonPropertyName("date")]
    public required string Date { get; set; }
    
    [JsonPropertyName("guestsNumber")]
    public required string GuestsNumber { get; set; }
 
    [JsonPropertyName("timeFrom")]
    public required string TimeFrom { get; set; }
 
    [JsonPropertyName("timeTo")]
    public required string TimeTo { get; set; }
}