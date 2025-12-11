using Dashboard.Models;
using System.Collections.Concurrent;

namespace Dashboard.Services;

/// <summary>
/// Service to manage production tracking data in memory
/// </summary>
public class ProductionTrackingService
{
    // Using ConcurrentDictionary for thread safety since this will be a singleton
    private readonly ConcurrentDictionary<string, ProductionData> _store = new();
    
    /// <summary>
    /// Gets production data for a component. Returns new default data if none exists.
    /// </summary>
    public ProductionData GetProductionData(string componentGuid)
    {
        return _store.GetOrAdd(componentGuid, guid => new ProductionData { ComponentGuid = guid });
    }
    
    /// <summary>
    /// Updates production data for a component
    /// </summary>
    public void UpdateProductionData(ProductionData data)
    {
        if (string.IsNullOrEmpty(data.ComponentGuid)) return;
        
        _store.AddOrUpdate(data.ComponentGuid, data, (key, oldValue) => data);
    }
}
