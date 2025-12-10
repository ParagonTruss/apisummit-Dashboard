using Dashboard.Models;

using System.Text.Json;

namespace Dashboard.Services;

/// <summary>
/// Service for managing dashboard state and persistence via localStorage
/// </summary>
public class DashboardStateService
{

    private readonly ILogger<DashboardStateService> _logger;
    private const string StorageKey = "paragon_dashboards";
    
    private List<DashboardConfiguration>? _dashboards;
    private DashboardConfiguration? _currentDashboard;
    
    public event Action? OnDashboardChanged;
    
    public DashboardStateService(ILogger<DashboardStateService> logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Gets the current active dashboard
    /// </summary>
    public DashboardConfiguration? CurrentDashboard => _currentDashboard;
    
    /// <summary>
    /// Loads all dashboards from localStorage
    /// </summary>
    public async Task<List<DashboardConfiguration>> LoadDashboardsAsync()
    {
        if (_dashboards != null)
            return _dashboards;
        
        // LocalStorage loading removed
        // try
        // {
        //     var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        //     
        //     if (!string.IsNullOrEmpty(json))
        //     {
        //         _dashboards = JsonSerializer.Deserialize<List<DashboardConfiguration>>(json, 
        //             new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        //     }
        //     else
        //     {
        //         _dashboards = new List<DashboardConfiguration>();
        //     }
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Failed to load dashboards from localStorage");
        //     _dashboards = new List<DashboardConfiguration>();
        // }
        
        if (_dashboards == null)
        {
            _dashboards = new List<DashboardConfiguration>();
        }
        
        return _dashboards;
    }
    
    /// <summary>
    /// Gets or creates a default dashboard
    /// </summary>
    public async Task<DashboardConfiguration> GetOrCreateDefaultDashboardAsync()
    {
        var dashboards = await LoadDashboardsAsync();
        
        if (dashboards.Count == 0)
        {
            var defaultDashboard = new DashboardConfiguration
            {
                Name = "My Dashboard",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            dashboards.Add(defaultDashboard);
            await SaveDashboardsAsync();
        }
        
        _currentDashboard = dashboards.First();
        return _currentDashboard;
    }
    
    /// <summary>
    /// Sets the current dashboard
    /// </summary>
    public async Task SetCurrentDashboardAsync(string dashboardId)
    {
        var dashboards = await LoadDashboardsAsync();
        _currentDashboard = dashboards.FirstOrDefault(d => d.Id == dashboardId);
        OnDashboardChanged?.Invoke();
    }
    
    /// <summary>
    /// Adds a widget to the current dashboard
    /// </summary>
    public async Task AddWidgetAsync(WidgetConfig widget)
    {
        if (_currentDashboard == null)
            await GetOrCreateDefaultDashboardAsync();
        
        _currentDashboard!.Widgets.Add(widget);
        _currentDashboard.UpdatedAt = DateTime.UtcNow;
        
        await SaveDashboardsAsync();
        OnDashboardChanged?.Invoke();
    }
    
    /// <summary>
    /// Updates a widget in the current dashboard
    /// </summary>
    public async Task UpdateWidgetAsync(WidgetConfig widget)
    {
        if (_currentDashboard == null) return;
        
        var index = _currentDashboard.Widgets.FindIndex(w => w.Id == widget.Id);
        if (index >= 0)
        {
            _currentDashboard.Widgets[index] = widget;
            _currentDashboard.UpdatedAt = DateTime.UtcNow;
            await SaveDashboardsAsync();
            OnDashboardChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Removes a widget from the current dashboard
    /// </summary>
    public async Task RemoveWidgetAsync(string widgetId)
    {
        if (_currentDashboard == null) return;
        
        _currentDashboard.Widgets.RemoveAll(w => w.Id == widgetId);
        _currentDashboard.UpdatedAt = DateTime.UtcNow;
        
        await SaveDashboardsAsync();
        OnDashboardChanged?.Invoke();
    }
    
    /// <summary>
    /// Updates widget position
    /// </summary>
    public async Task UpdateWidgetPositionAsync(string widgetId, int row, int column)
    {
        if (_currentDashboard == null) return;
        
        var widget = _currentDashboard.Widgets.FirstOrDefault(w => w.Id == widgetId);
        if (widget != null)
        {
            widget.Row = row;
            widget.Column = column;
            _currentDashboard.UpdatedAt = DateTime.UtcNow;
            await SaveDashboardsAsync();
        }
    }
    
    /// <summary>
    /// Updates widget size
    /// </summary>
    public async Task UpdateWidgetSizeAsync(string widgetId, int width, int height)
    {
        if (_currentDashboard == null) return;
        
        var widget = _currentDashboard.Widgets.FirstOrDefault(w => w.Id == widgetId);
        if (widget != null)
        {
            widget.Width = width;
            widget.Height = height;
            _currentDashboard.UpdatedAt = DateTime.UtcNow;
            await SaveDashboardsAsync();
        }
    }
    
    /// <summary>
    /// Creates a new dashboard
    /// </summary>
    public async Task<DashboardConfiguration> CreateDashboardAsync(string name)
    {
        var dashboard = new DashboardConfiguration
        {
            Name = name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        var dashboards = await LoadDashboardsAsync();
        dashboards.Add(dashboard);
        
        await SaveDashboardsAsync();
        
        _currentDashboard = dashboard;
        OnDashboardChanged?.Invoke();
        
        return dashboard;
    }
    
    /// <summary>
    /// Deletes a dashboard
    /// </summary>
    public async Task DeleteDashboardAsync(string dashboardId)
    {
        var dashboards = await LoadDashboardsAsync();
        dashboards.RemoveAll(d => d.Id == dashboardId);
        
        if (_currentDashboard?.Id == dashboardId)
        {
            _currentDashboard = dashboards.FirstOrDefault();
        }
        
        await SaveDashboardsAsync();
        OnDashboardChanged?.Invoke();
    }
    
    /// <summary>
    /// Saves all dashboards to localStorage
    /// </summary>
    private async Task SaveDashboardsAsync()
    {
        _logger.LogDebug("Saved dashboards (in-memory only)");
        await Task.CompletedTask;
        // try
        // {
        //     var json = JsonSerializer.Serialize(_dashboards, new JsonSerializerOptions 
        //     { 
        //         WriteIndented = false,
        //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //     });
        //     
        //     await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        //     _logger.LogDebug("Saved dashboards to localStorage");
        // }
        // catch (Exception ex)
        // {
        //     _logger.LogError(ex, "Failed to save dashboards to localStorage");
        // }
    }
}
