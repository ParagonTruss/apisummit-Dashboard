using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents lumber details
/// </summary>
public class Lumber
{
    /// <summary>
    /// The species of the lumber
    /// </summary>
    [JsonPropertyName("species")]
    public string Species { get; set; } = string.Empty;

    /// <summary>
    /// The grade of the lumber
    /// </summary>
    [JsonPropertyName("grade")]
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Nominal thickness
    /// </summary>
    [JsonPropertyName("nominalThickness")]
    public string NominalThickness { get; set; }= string.Empty;

    /// <summary>
    /// Nominal width
    /// </summary>
    [JsonPropertyName("nominalWidth")]
    public string NominalWidth { get; set; }= string.Empty;


    /// <summary>
    /// Actual thickness
    /// </summary>
    [JsonPropertyName("actualThickness")]
    public double ActualThickness { get; set; }

    /// <summary>
    /// Actual width
    /// </summary>
    [JsonPropertyName("actualWidth")]
    public double ActualWidth { get; set; }

    /// <summary>
    /// Structure type
    /// </summary>
    [JsonPropertyName("structure")]
    public string Structure { get; set; } = string.Empty;

    /// <summary>
    /// Treatment type
    /// </summary>
    [JsonPropertyName("treatmentType")]
    public string TreatmentType { get; set; } = string.Empty;
}
