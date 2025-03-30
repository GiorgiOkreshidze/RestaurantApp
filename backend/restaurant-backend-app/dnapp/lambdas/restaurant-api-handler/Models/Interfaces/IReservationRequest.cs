using System.Text.Json.Serialization;

namespace Function.Models.Interfaces;

public interface IReservationRequest
{
    // if no id, we are generating it otherwise, we update record based on the provided id.
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("locationId")]
    public string LocationId { get; set; }
    
    [JsonPropertyName("tableId")]
    public string TableId { get; set; }
 
    [JsonPropertyName("date")]
    public string Date { get; set; }
    
    [JsonPropertyName("guestsNumber")]
    public string GuestsNumber { get; set; }
 
    [JsonPropertyName("timeFrom")]
    public string TimeFrom { get; set; }
 
    [JsonPropertyName("timeTo")]
    public string TimeTo { get; set; }
}