using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a project in the Paragon system
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the project
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the project
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// List of GUIDs for component designs in the project
    /// </summary>
    [JsonPropertyName("componentDesignGuids")]
    public List<string> ComponentDesignGuids { get; set; } = new List<string>();
}
