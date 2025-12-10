namespace Dashboard.Models;

/// <summary>
/// Represents an API endpoint extracted from the OpenAPI document
/// </summary>
public class ApiEndpointInfo
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string Tag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<ApiParameterInfo> Parameters { get; set; } = new();
    public SchemaInfo? ResponseSchema { get; set; }
    
    public string DisplayName => $"{Tag}: {Path}";
}

/// <summary>
/// Represents a parameter for an API endpoint
/// </summary>
public class ApiParameterInfo
{
    public string Name { get; set; } = string.Empty;
    public string In { get; set; } = "query"; // path, query, header
    public bool Required { get; set; }
    public string Type { get; set; } = "string";
    public string? Format { get; set; }
}

/// <summary>
/// Represents a schema from the OpenAPI document
/// </summary>
public class SchemaInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "object";
    public bool IsArray { get; set; }
    public string? ItemType { get; set; }
    public List<SchemaPropertyInfo> Properties { get; set; } = new();
}

/// <summary>
/// Represents a property within a schema
/// </summary>
public class SchemaPropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public string? Format { get; set; }
    public bool Required { get; set; }
}
