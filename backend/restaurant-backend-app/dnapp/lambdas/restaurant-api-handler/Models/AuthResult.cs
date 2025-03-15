using System.Text.Json.Serialization;

namespace Function.Models
{
    public class AuthResult
    {
        [JsonPropertyName("idToken")]
        public required string IdToken { get; set; }
        [JsonPropertyName("refreshToken")]
        public required string RefreshToken { get; set; }

        [JsonPropertyName("accessToken")]
        public required string AccessToken { get; set; }

        [JsonPropertyName("expiresIn")]
        public int ExpiresIn { get; set; }
    }
}
