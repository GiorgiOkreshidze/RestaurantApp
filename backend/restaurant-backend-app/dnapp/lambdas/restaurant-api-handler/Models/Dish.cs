using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.DataModel;

namespace Function.Models;

public class Dish
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
    
    [DynamoDBProperty("isPopular")]
    [JsonIgnore]
    public bool IsPopular { get; set; }
    
    [DynamoDBProperty("locationId")]
    [JsonIgnore]
    public string? LocationId { get; set; }
}