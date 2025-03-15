using System.Text.Json.Serialization;

namespace Function.Models;

public class PopularDish
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("price")]
    public required string Price { get; set; }
    
    [JsonPropertyName("weight")]
    public required string Weight { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public required string ImageUrl { get; set; }
}