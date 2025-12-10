using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a member in a component
/// </summary>
public class Member
{
    /// <summary>
    /// The lumber used for this member
    /// </summary>
    [JsonPropertyName("lumber")]

    public Lumber? Lumber { get; set; }

    /// <summary>
    /// Overall length of the member
    /// </summary>
    [JsonPropertyName("overallLength")]
    public double Length { get; set; }
}
