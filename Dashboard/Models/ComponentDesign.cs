using System.Text.Json.Serialization;

namespace Dashboard.Models;
/// <summary>
/// Represents a component design
/// </summary>
public class ComponentDesignResponse
{
    /// <summary>
    /// The component details
    /// </summary>
    [JsonPropertyName("componentDesign")]
    public ComponentDesign? ComponentDesign { get; set; }
}


/// <summary>
/// Represents a component design
/// </summary>
public class ComponentDesign
{
    /// <summary>
    /// The component details
    /// </summary>
    [JsonPropertyName("component")]
    public Component? Component { get; set; }
}
