using System.Text.Json.Serialization;

namespace Function.Models.Requests
{
    public class SignOutRequest
    {
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
    }
}
