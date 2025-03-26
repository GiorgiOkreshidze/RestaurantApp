using System.Text.Json.Serialization;

namespace SimpleLambdaFunction.Models.Status;

public enum StatusEnum
{
    [JsonPropertyName("on Stop")]
    OnStop = 1,
    Discount = 2,
    Active = 3
}