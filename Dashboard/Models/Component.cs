using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a component
/// </summary>
public class Component
{
    /// <summary>
    /// List of members in the component
    /// </summary>
    [JsonPropertyName("members")]
    public List<Member> Members { get; set; } = new List<Member>();

    /// <summary>
    /// List of plate pairs in the component
    /// </summary>
    [JsonPropertyName("platePairs")]
    public List<PlatePair> PlatePairs { get; set; } = new List<PlatePair>();

    /// <summary>
    /// The number of plies in the component
    /// </summary>
    [JsonPropertyName("numberOfPlies")]
    public int NumberOfPlies { get; set; }

    /// <summary>
    /// The name of the component
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
