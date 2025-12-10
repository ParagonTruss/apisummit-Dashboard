using Dashboard.Models;
using System.Text.Json;

namespace Dashboard.Services;

/// <summary>
/// Service for parsing the OpenAPI document and extracting endpoint information
/// </summary>
public class OpenApiService
{
    private readonly ILogger<OpenApiService> _logger;
    private List<ApiEndpointInfo>? _endpoints;
    private Dictionary<string, SchemaInfo>? _schemas;
    private readonly string _openApiPath;
    
    public OpenApiService(ILogger<OpenApiService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        // The v1.json is at the repo root, one level up from Dashboard
        _openApiPath = Path.Combine(env.ContentRootPath, "..", "v1.json");
    }
    
    /// <summary>
    /// Gets all available API endpoints (GET only for dashboard widgets)
    /// </summary>
    public async Task<List<ApiEndpointInfo>> GetEndpointsAsync()
    {
        if (_endpoints == null)
        {
            await ParseOpenApiDocumentAsync();
        }
        return _endpoints ?? new List<ApiEndpointInfo>();
    }
    
    /// <summary>
    /// Gets all available schemas from the OpenAPI document
    /// </summary>
    public async Task<Dictionary<string, SchemaInfo>> GetSchemasAsync()
    {
        if (_schemas == null)
        {
            await ParseOpenApiDocumentAsync();
        }
        return _schemas ?? new Dictionary<string, SchemaInfo>();
    }
    
    /// <summary>
    /// Gets endpoints grouped by their tag (category)
    /// </summary>
    public async Task<Dictionary<string, List<ApiEndpointInfo>>> GetEndpointsByTagAsync()
    {
        var endpoints = await GetEndpointsAsync();
        return endpoints.GroupBy(e => e.Tag)
                       .ToDictionary(g => g.Key, g => g.ToList());
    }
    
    /// <summary>
    /// Gets schema info for a specific response type
    /// </summary>
    public async Task<SchemaInfo?> GetSchemaAsync(string schemaName)
    {
        var schemas = await GetSchemasAsync();
        return schemas.TryGetValue(schemaName, out var schema) ? schema : null;
    }
    
    private async Task ParseOpenApiDocumentAsync()
    {
        try
        {
            if (!File.Exists(_openApiPath))
            {
                _logger.LogWarning("OpenAPI document not found at {Path}", _openApiPath);
                _endpoints = new List<ApiEndpointInfo>();
                _schemas = new Dictionary<string, SchemaInfo>();
                return;
            }
            
            var json = await File.ReadAllTextAsync(_openApiPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            
            _schemas = ParseSchemas(root);
            _endpoints = ParseEndpoints(root);
            
            _logger.LogInformation("Parsed {EndpointCount} endpoints and {SchemaCount} schemas from OpenAPI document", 
                _endpoints.Count, _schemas.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse OpenAPI document");
            _endpoints = new List<ApiEndpointInfo>();
            _schemas = new Dictionary<string, SchemaInfo>();
        }
    }
    
    private Dictionary<string, SchemaInfo> ParseSchemas(JsonElement root)
    {
        var schemas = new Dictionary<string, SchemaInfo>();
        
        if (root.TryGetProperty("components", out var components) &&
            components.TryGetProperty("schemas", out var schemasElement))
        {
            foreach (var schema in schemasElement.EnumerateObject())
            {
                var schemaInfo = new SchemaInfo
                {
                    Name = schema.Name,
                    Type = schema.Value.TryGetProperty("type", out var type) ? type.GetString() ?? "object" : "object"
                };
                
                // Parse properties
                if (schema.Value.TryGetProperty("properties", out var properties))
                {
                    var requiredProps = new HashSet<string>();
                    if (schema.Value.TryGetProperty("required", out var required))
                    {
                        foreach (var r in required.EnumerateArray())
                        {
                            requiredProps.Add(r.GetString() ?? "");
                        }
                    }
                    
                    foreach (var prop in properties.EnumerateObject())
                    {
                        var propInfo = new SchemaPropertyInfo
                        {
                            Name = prop.Name,
                            Required = requiredProps.Contains(prop.Name)
                        };
                        
                        if (prop.Value.TryGetProperty("type", out var propType))
                        {
                            // Handle array types like ["null", "string"]
                            if (propType.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var t in propType.EnumerateArray())
                                {
                                    var typeStr = t.GetString();
                                    if (typeStr != "null")
                                    {
                                        propInfo.Type = typeStr ?? "string";
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                propInfo.Type = propType.GetString() ?? "string";
                            }
                        }
                        
                        if (prop.Value.TryGetProperty("format", out var format))
                        {
                            propInfo.Format = format.GetString();
                        }
                        
                        schemaInfo.Properties.Add(propInfo);
                    }
                }
                
                schemas[schema.Name] = schemaInfo;
            }
        }
        
        return schemas;
    }
    
    private List<ApiEndpointInfo> ParseEndpoints(JsonElement root)
    {
        var endpoints = new List<ApiEndpointInfo>();
        
        if (root.TryGetProperty("paths", out var paths))
        {
            foreach (var path in paths.EnumerateObject())
            {
                foreach (var method in path.Value.EnumerateObject())
                {
                    // Only include GET endpoints for dashboard widgets
                    if (method.Name.ToUpper() != "GET")
                        continue;
                    
                    var endpoint = new ApiEndpointInfo
                    {
                        Path = path.Name,
                        Method = method.Name.ToUpper()
                    };
                    
                    // Get tag/category
                    if (method.Value.TryGetProperty("tags", out var tags) && tags.GetArrayLength() > 0)
                    {
                        endpoint.Tag = tags[0].GetString() ?? "Other";
                    }
                    
                    // Get parameters
                    if (method.Value.TryGetProperty("parameters", out var parameters))
                    {
                        foreach (var param in parameters.EnumerateArray())
                        {
                            var paramInfo = new ApiParameterInfo
                            {
                                Name = param.TryGetProperty("name", out var name) ? name.GetString() ?? "" : "",
                                In = param.TryGetProperty("in", out var inProp) ? inProp.GetString() ?? "query" : "query",
                                Required = param.TryGetProperty("required", out var req) && req.GetBoolean()
                            };
                            
                            if (param.TryGetProperty("schema", out var schema))
                            {
                                if (schema.TryGetProperty("type", out var schemaType))
                                {
                                    paramInfo.Type = schemaType.GetString() ?? "string";
                                }
                                if (schema.TryGetProperty("format", out var format))
                                {
                                    paramInfo.Format = format.GetString();
                                }
                            }
                            
                            endpoint.Parameters.Add(paramInfo);
                        }
                    }
                    
                    // Get response schema
                    if (method.Value.TryGetProperty("responses", out var responses) &&
                        responses.TryGetProperty("200", out var ok) &&
                        ok.TryGetProperty("content", out var content))
                    {
                        // Try to get application/json schema
                        JsonElement? schemaRef = null;
                        if (content.TryGetProperty("application/json", out var jsonContent))
                        {
                            if (jsonContent.TryGetProperty("schema", out var s))
                                schemaRef = s;
                        }
                        else if (content.TryGetProperty("text/json", out var textJsonContent))
                        {
                            if (textJsonContent.TryGetProperty("schema", out var s))
                                schemaRef = s;
                        }
                        
                        if (schemaRef.HasValue)
                        {
                            endpoint.ResponseSchema = ParseResponseSchema(schemaRef.Value);
                        }
                    }
                    
                    endpoints.Add(endpoint);
                }
            }
        }
        
        return endpoints.OrderBy(e => e.Tag).ThenBy(e => e.Path).ToList();
    }
    
    private SchemaInfo ParseResponseSchema(JsonElement schema)
    {
        var info = new SchemaInfo();
        
        // Handle $ref
        if (schema.TryGetProperty("$ref", out var refProp))
        {
            var refPath = refProp.GetString() ?? "";
            info.Name = refPath.Split('/').LastOrDefault() ?? "";
            info.Type = "object";
        }
        // Handle array type
        else if (schema.TryGetProperty("type", out var type) && type.GetString() == "array")
        {
            info.IsArray = true;
            info.Type = "array";
            
            if (schema.TryGetProperty("items", out var items))
            {
                if (items.TryGetProperty("$ref", out var itemRef))
                {
                    info.ItemType = itemRef.GetString()?.Split('/').LastOrDefault();
                }
            }
        }
        else
        {
            info.Type = schema.TryGetProperty("type", out var t) ? t.GetString() ?? "object" : "object";
        }
        
        return info;
    }
}
