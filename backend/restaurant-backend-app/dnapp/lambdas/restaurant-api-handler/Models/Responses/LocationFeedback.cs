using System.Text.Json.Serialization;

namespace Function.Models.Responses;

public class LocationFeedback
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("rate")]
    public required string Rate { get; set; }
    
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