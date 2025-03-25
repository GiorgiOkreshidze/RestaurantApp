using System.Text.Json.Serialization;

namespace Function.Models.User;

public class User
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("firstName")]
    public required string FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public required string LastName { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("role")]
    public required Roles Role { get; set; }
    
    [JsonPropertyName("locationId")]
    public string? LocationId { get; set; }
    
    [JsonPropertyName("createdAt")]
    public required string CreatedAt { get; set; }
}