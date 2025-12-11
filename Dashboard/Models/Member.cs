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
    /// The geometry of the member
    /// </summary>
    [JsonPropertyName("geometry")]
    public List<Point> Geometry { get; set; } = new List<Point>();

    /// <summary>
    /// Overall length of the member, calculated from geometry
    /// </summary>
    [JsonIgnore]
    public double Length
    {
        get
        {
            if (Geometry == null || Geometry.Count < 2) return 0;

            double maxDistance = 0;
            // Iterate all unique pairs to find max distance
            for (int i = 0; i < Geometry.Count; i++)
            {
                for (int j = i + 1; j < Geometry.Count; j++)
                {
                    var p1 = Geometry[i];
                    var p2 = Geometry[j];
                    
                    var dx = p1.X - p2.X;
                    var dy = p1.Y - p2.Y;
                    var distance = Math.Sqrt(dx * dx + dy * dy);
                    
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                }
            }
            return maxDistance;
        }
    }
}
