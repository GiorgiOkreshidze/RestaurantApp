using System.Text.Json.Serialization;

namespace SimpleLambdaFunction.Models.Nutritient;

public class NutritientsDto
{
    [JsonPropertyName("calories")]
    public string? Calories { get; set; }
    [JsonPropertyName("protein")]
    public string? Protein { get; set; }
    [JsonPropertyName("fats")]
    public string? Fats { get; set; }
    [JsonPropertyName("carbohydrates")]
    public string? Carbohydrates { get; set; }
    [JsonPropertyName("vitamins")]
    public string? Vitamins { get; set; }
}