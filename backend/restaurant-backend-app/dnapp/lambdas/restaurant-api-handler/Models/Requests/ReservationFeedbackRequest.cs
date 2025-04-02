using System.Text.Json.Serialization;

namespace Function.Models.Requests;

public class ReservationFeedbackRequest
{
    [JsonPropertyName("reservationId")]
    public string ReservationId { get; set; }
    
    [JsonPropertyName("cuisineComment")]
    public string? CuisineComment { get; set; }
    
    [JsonPropertyName("serviceComment")]
    public string? ServiceComment { get; set; }
    
    [JsonPropertyName("cuisineRating")]
    public string? CuisineRating { get; set; }
    
    [JsonPropertyName("serviceRating")]
    public string? ServiceRating { get; set; }
}