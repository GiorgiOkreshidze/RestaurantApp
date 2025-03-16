using System.Text.Json.Serialization;

namespace Function.Models
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
    }
}
