using System.Text.Json.Serialization;
using Function.Models.Interfaces;

namespace Function.Models.Requests;

public class WaiterReservationRequest : IReservationRequest
{
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
    
    [JsonPropertyName("clientType")]
    public required string ClientType { get; set; }
    
    [JsonPropertyName("customerName")]
    public required string CustomerName { get; set; }
}