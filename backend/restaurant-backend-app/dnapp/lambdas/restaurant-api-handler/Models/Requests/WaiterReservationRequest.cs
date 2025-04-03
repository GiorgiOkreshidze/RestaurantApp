using System.Text.Json.Serialization;
using Function.Models.Requests.Base;
using Function.Models.User;

namespace Function.Models.Requests;

public class WaiterReservationRequest : BaseReservationRequest
{
    [JsonPropertyName("clientType")]
    public required ClientType ClientType { get; set; }
    
    [JsonPropertyName("customerId")]
    public string? CustomerId { get; set; }
}