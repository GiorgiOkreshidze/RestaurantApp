using Function.Models.User;
using System.Text.Json.Serialization;

namespace Function.Models.Responses
{
    public class UserResponse
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("firstName")]
        public required string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public required string LastName { get; set; }

        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("imageUrl")]
        public required string ImageUrl { get; set; }

        [JsonPropertyName("role")]
        public required string Role { get; set; }

    }
}
