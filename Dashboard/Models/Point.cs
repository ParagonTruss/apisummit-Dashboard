using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a 2D point in geometry
/// </summary>
public class Point
{
    /// <summary>
    /// X coordinate
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>
    /// Y coordinate
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }
}
