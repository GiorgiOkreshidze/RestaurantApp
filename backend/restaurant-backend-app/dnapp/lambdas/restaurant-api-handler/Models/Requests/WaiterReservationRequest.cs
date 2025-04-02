using System.Text.Json.Serialization;
using Function.Models.Requests.Base;
using Function.Models.User;

namespace Function.Models.Requests;

public class WaiterReservationRequest : BaseReservationRequest
{
    [JsonPropertyName("clientType")]
    public required ClientType ClientType { get; set; }
    
    [JsonPropertyName("customerName")]
    public required string CustomerName { get; set; }
}