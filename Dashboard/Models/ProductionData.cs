using System;
using System.Collections.Generic;

namespace Dashboard.Models;

/// <summary>
/// detailed production data for a single component
/// </summary>
public class ProductionData
{
    /// <summary>
    /// ID of the component this data belongs to
    /// </summary>
    public string ComponentGuid { get; set; } = string.Empty;

    /// <summary>
    /// Current status of production
    /// </summary>
    public ProductionStatus Status { get; set; } = ProductionStatus.Pending;

    /// <summary>
    /// Hours spent in each stage
    /// </summary>
    public Dictionary<ProductionStatus, double> TimeInStages { get; set; } = new();

    /// <summary>
    /// Total labor cost
    /// </summary>
    public decimal LaborCost { get; set; }

    /// <summary>
    /// Projected start date/time
    /// </summary>
    public DateTime? ProjectedStartTime { get; set; }

    /// <summary>
    /// Projected end date/time
    /// </summary>
    public DateTime? ProjectedEndTime { get; set; }

    /// <summary>
    /// Projected production day (just date component usually, but DateTime covers it)
    /// </summary>
    public DateTime? ProjectedProductionDate { get; set; }
}
