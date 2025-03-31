using System.Text.Json.Serialization;

namespace Function.Models.Requests;

/*
 * {
  "cuisineComment": "Good food",
  "cuisineRating": "4",
  "reservationId": "672846d5c951184d705b65d7",
  "serviceComment": "Good service, good atmosphere",
  "serviceRating": "5"
}
*/
public class FeedbackRequest
{
    [JsonPropertyName("reservationId")]
    public string ReservationId { get; set; }
    
    [JsonPropertyName("cuisineComment")]
    public string? CuisineComment { get; set; }
    
    [JsonPropertyName("serviceComment")]
    public string? ServiceComment { get; set; }
    
    [JsonPropertyName("cuisineRating")]
    public string CuisineRating { get; set; }
    
    [JsonPropertyName("serviceRating")]
    public string ServiceRating { get; set; }
}