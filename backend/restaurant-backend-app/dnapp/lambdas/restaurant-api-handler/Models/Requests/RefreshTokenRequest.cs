using System.Text.Json.Serialization;

namespace Function.Models.Requests
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
    }
}
