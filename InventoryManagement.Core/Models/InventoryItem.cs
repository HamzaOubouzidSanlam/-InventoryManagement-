namespace InventoryManagement.Core.Models;

public sealed class InventoryItem
{
    public required int ItemId { get; init; }
    public required string ItemName { get; init; }
    public int CurrentQuantity { get; set; }
    public DateTime LastUpdated { get; set; }
}
