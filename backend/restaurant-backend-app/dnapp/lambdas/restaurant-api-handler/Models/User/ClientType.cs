using System.Text.Json.Serialization;

namespace Function.Models.User;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClientType
{
    CUSTOMER,
    VISITOR
}