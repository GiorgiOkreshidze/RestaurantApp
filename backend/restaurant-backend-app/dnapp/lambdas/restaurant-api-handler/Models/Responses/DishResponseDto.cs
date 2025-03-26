using System.Collections.Generic;
using System.Text.Json.Serialization;
using SimpleLambdaFunction.Models.Nutritient;

namespace Function.Models.Responses;

public class DishResponseDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public required string ImageUrl { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("price")]
    public required string Price { get; set; }
    
    [JsonPropertyName("weight")]
    public required string Weight { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("category")]
    public required IEnumerable<string> Category { get; set; }
    
    [JsonPropertyName("nutritients")]
    public required IEnumerable<NutritientsDto>? Nutritients { get; set; }
}