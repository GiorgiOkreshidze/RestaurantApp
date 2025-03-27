using System.Text.Json.Serialization;

namespace Function.Models.Responses;

public class AllDishResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public required string PreviewImageUrl { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("price")]
    public required string Price { get; set; }
    
    [JsonPropertyName("weight")]
    public required string Weight { get; set; }

    [JsonPropertyName("dishType")]
    public required string DishType { get; set; }
    
    [JsonPropertyName("state")]
    public required string State { get; set; }
   
    [JsonPropertyName("isPopular")]
    public bool IsPopular { get; set; }
}