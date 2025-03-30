using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Function.Models.Reservations;

public class Reservation
{
    [JsonPropertyName("id")]
    [DynamoDBProperty("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("date")]
    public required string Date { get; set; }
    
    [JsonPropertyName("feedbackId")]
    public string FeedbackId { get; set; }
    
    [JsonPropertyName("guestsNumber")]
    public required string GuestsNumber { get; set; }
    
    [JsonPropertyName("locationId")]
    public required string LocationId { get; set; }
    
    [JsonPropertyName("locationAddress")]
    public required string LocationAddress { get; set; }

    [JsonPropertyName("preOrder")]
    public required string PreOrder { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("tableId")]
    public required string TableId { get; set; }

    [JsonPropertyName("tableNumber")]
    public required string TableNumber { get; set; }

    [JsonPropertyName("timeFrom")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public required string TimeFrom { get; set; }

    [JsonPropertyName("timeTo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] 
    public required string TimeTo { get; set; }
    
    [JsonPropertyName("timeSlot")]
    public required string TimeSlot { get; set; }
    
    [JsonPropertyName("userInfo")]
    public string? UserInfo { get; set; }
    
    [JsonPropertyName("userEmail")]
    public string? UserEmail { get; set; }
    
    [JsonPropertyName("waiterId")]
    [JsonIgnore]
    public string? WaiterId { get; set; }

    [JsonPropertyName("createdAt")]
    public required string CreatedAt { get; set; }
    
    [JsonPropertyName("clientType")]
    public string? ClientType { get; set; }
}