using System.Text.Json;
using Dashboard.Models;

namespace Dashboard.Services;

/// <summary>
/// HTTP client for making requests to the Paragon API
/// </summary>
public class ParagonApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ParagonApiClient> _logger;
    
    public ParagonApiClient(HttpClient httpClient, ILogger<ParagonApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    /// <summary>
    /// Makes a GET request to the specified endpoint and returns the response as JSON
    /// </summary>
    public async Task<JsonElement?> GetAsync(string path, Dictionary<string, string>? queryParams = null)
    {
        try
        {
            var url = path;
            
            // Add query parameters
            if (queryParams != null && queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
                    .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                
                if (!string.IsNullOrEmpty(queryString))
                {
                    url = $"{path}?{queryString}";
                }
            }
            
            _logger.LogInformation("Making GET request to {Url}", url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }
            
            return JsonDocument.Parse(content).RootElement.Clone();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling {Path}: {Message}", path, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API endpoint {Path}", path);
            throw;
        }
    }
    
    /// <summary>
    /// Checks if the API is reachable
    /// </summary>
    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            // Try to get projects as a simple health check
            var response = await _httpClient.GetAsync("/api/public/projects");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Gets the list of available projects
    /// </summary>
    public async Task<List<Project>> GetProjectsAsync()
    {
        try
        {
            var json = await GetAsync("/api/public/projects");
            
            if (json.HasValue)
            {
                return JsonSerializer.Deserialize<List<Project>>(json.Value.GetRawText(), 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Project>();
            }
            
            return new List<Project>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch projects");
            return new List<Project>();
        }
    }

    /// <summary>
    /// Gets a component design by its GUID
    /// </summary>
    public async Task<ComponentDesignResponse?> GetComponentDesignAsync(string componentDesignGuid)
    {
        try
        {
            var json = await GetAsync($"/api/ComponentDesigns/{componentDesignGuid}");
            
            if (json.HasValue)
            {
                var jsonstr = json.Value.GetRawText();
                var serialized = JsonSerializer.Deserialize<ComponentDesignResponse>(json.Value.GetRawText(), 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return serialized;
            }
            
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch component design for {Guid}", componentDesignGuid);
            return null;
        }
    }

    /// <summary>
    /// Gets lumber prices for all stock lengths given the lumber specifications
    /// </summary>
    public async Task<List<LumberPriceResponse>> GetLumberPricesAsync(LumberPriceRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/LumberPrices/mostRecent/forAllStockLengths", request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Try to deserialize as list
                var prices = JsonSerializer.Deserialize<List<LumberPriceResponse>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                return prices ?? new List<LumberPriceResponse>();
            }
            else
            {
                _logger.LogWarning("Failed to fetch prices. Status: {StatusCode}", response.StatusCode);
                return new List<LumberPriceResponse>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch lumber prices");
            return new List<LumberPriceResponse>();
        }
    }

    /// <summary>
    /// Gets plate type properties including thickness
    /// </summary>
    public async Task<PlateTypeProperties?> GetPlateTypePropertiesAsync(string plateType)
    {
        try
        {
            var json = await GetAsync($"/api/PlatePairs/getPlateTypeProperties/{plateType}");
            
            if (json.HasValue)
            {
                var properties = JsonSerializer.Deserialize<PlateTypeProperties>(json.Value.GetRawText(), 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return properties;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch plate type properties for {PlateType}", plateType);
            return null;
        }
    }
}
