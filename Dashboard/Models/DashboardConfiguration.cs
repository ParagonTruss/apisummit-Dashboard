using System.Text.Json.Serialization;

namespace Dashboard.Models;

/// <summary>
/// Represents a complete dashboard configuration with all widgets
/// </summary>
public class DashboardConfiguration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "My Dashboard";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<WidgetConfig> Widgets { get; set; } = new();
}

/// <summary>
/// Configuration for a single widget on the dashboard
/// </summary>
public class WidgetConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "New Widget";
    public WidgetType Type { get; set; } = WidgetType.DataTable;
    
    // Grid position (CSS Grid based - 12 column layout)
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public int Width { get; set; } = 6; // Number of columns (1-12)
    public int Height { get; set; } = 2; // Number of rows
    
    // API Configuration
    public string? EndpointPath { get; set; }
    public string? EndpointTemplate { get; set; } // The original template path (e.g. /api/users/{id})
    public string? EndpointMethod { get; set; } = "GET";
    public List<string> DisplayFields { get; set; } = new();
    public Dictionary<string, string> Filters { get; set; } = new();
    
    // Chart-specific settings
    public string? XAxisField { get; set; }
    public string? YAxisField { get; set; }
    public string? ValueField { get; set; } // For KPI cards
    public string? LabelField { get; set; } // For pie charts
    
    // Refresh settings
    public int RefreshIntervalSeconds { get; set; } = 0; // 0 = no auto-refresh
}

/// <summary>
/// Types of widgets available in the dashboard
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WidgetType
{
    DataTable,
    BarChart,
    LineChart,
    PieChart,
    KpiCard
}
