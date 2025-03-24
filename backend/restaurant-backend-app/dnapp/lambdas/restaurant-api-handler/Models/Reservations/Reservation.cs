using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Function.Models;

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
    
    [JsonPropertyName("locationAddress")]
    public required string LocationAddress { get; set; }
    
    [JsonPropertyName("preOrder")]
    public required string PreOrder { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }

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
    public required string UserInfo { get; set; }
    
    [JsonIgnore]
    public string? WaiterEmail { get; set; }

    [JsonPropertyName("createdAt")]
    public required string CreatedAt { get; set; }
}