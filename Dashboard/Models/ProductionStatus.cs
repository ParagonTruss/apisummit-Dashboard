namespace Dashboard.Models;

/// <summary>
/// Represents the current production status of a component
/// </summary>
public enum ProductionStatus
{
    Pending,
    Cutting,
    Assembling,
    Packing,
    Shipped
}
