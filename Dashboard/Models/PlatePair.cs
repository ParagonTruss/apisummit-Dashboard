using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a plate pair (connector plate)
/// </summary>
public class PlatePair
{
    /// <summary>
    /// The type of the plate
    /// </summary>
    [JsonPropertyName("plateType")]
    public string PlateType { get; set; } = string.Empty;

    /// <summary>
    /// The width of the plate
    /// </summary>
    [JsonPropertyName("width")]
    public double Width { get; set; }

    /// <summary>
    /// The length of the plate
    /// </summary>
    [JsonPropertyName("length")]
    public double Length { get; set; }
}
