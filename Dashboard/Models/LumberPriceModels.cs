using System.Text.Json.Serialization;

namespace Dashboard.Models;

public class LumberPriceRequest
{
    [JsonPropertyName("actualThickness")]
    public string ActualThickness { get; set; } = string.Empty;

    [JsonPropertyName("actualWidth")]
    public string ActualWidth { get; set; } = string.Empty;

    [JsonPropertyName("grade")]
    public string Grade { get; set; } = string.Empty;

    [JsonPropertyName("species")]
    public string Species { get; set; } = string.Empty;

    [JsonPropertyName("structure")]
    public string Structure { get; set; } = string.Empty;

    [JsonPropertyName("treatmentType")]
    public string TreatmentType { get; set; } = string.Empty;

    [JsonPropertyName("NominalWidth")]
    public string NominalWidth { get; set; } = string.Empty;

    [JsonPropertyName("NominalThickness")]
    public string NominalThickness { get; set; } = string.Empty;
}

public class LumberPriceResponse
{
    // Assuming API returns a map or list of lengths and prices. 
    // Since exact shape isn't specific, I'll assume list of objects with length/cost based on "forAllStockLengths"
    
    [JsonPropertyName("length")]
    public double Length { get; set; }

    [JsonPropertyName("cost")]
    public decimal Cost { get; set; }
}
