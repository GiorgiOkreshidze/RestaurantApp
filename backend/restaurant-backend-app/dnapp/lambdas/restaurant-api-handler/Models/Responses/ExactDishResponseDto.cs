using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SimpleLambdaFunction.Models.Nutritient;

namespace Function.Models.Responses;

public class ExactDishResponseDto
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public required string ImageUrl { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("nutritients")]
    public IEnumerable<NutritientsDto>? Nutritients { get; set; }
    
    [JsonPropertyName("price")]
    public required string Price { get; set; }
    
    [JsonPropertyName("weight")]
    public required string Weight { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("category")]
    public required IEnumerable<string> Category { get; set; }
}