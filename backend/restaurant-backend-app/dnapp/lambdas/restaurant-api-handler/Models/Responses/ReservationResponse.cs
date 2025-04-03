using System.Text.Json.Serialization;

namespace Function.Models.Responses;

public class ReservationResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("date")]
    public required string Date { get; set; }
    
    [JsonPropertyName("guestsNumber")]
    public required string GuestsNumber { get; set; }
    
    [JsonPropertyName("locationAddress")]
    public required string LocationAddress { get; set; }
    
    [JsonPropertyName("locationId")]
    public required string LocationId { get; set; }
    
    [JsonPropertyName("preOrder")]
    public required string PreOrder { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("tableNumber")]
    public required string TableNumber { get; set; }
    
    [JsonPropertyName("timeFrom")]
    public required string TimeFrom { get; set; }
    
    [JsonPropertyName("timeSlot")]
    public required string TimeSlot { get; set; }
    
    [JsonPropertyName("userInfo")]
    public required string? UserInfo { get; set; }
    
    [JsonPropertyName("editableTill")]
    public required string EditableTill { get; set; } // Computed or set based on business rules
}