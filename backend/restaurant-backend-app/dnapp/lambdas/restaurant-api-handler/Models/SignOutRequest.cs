using System.Text.Json.Serialization;

namespace Function.Models
{
    public class SignOutRequest
    {
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }
    }
}
