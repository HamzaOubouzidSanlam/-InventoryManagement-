using InventoryManagement.Core.Models;

namespace InventoryManagement.Core.Services;

public sealed class FakeInventoryService : IInventoryService
{
    private readonly List<InventoryItem> inventory =
    [
        new InventoryItem
        {
            ItemId = 1,
            ItemName = "Business Laptop",
            CurrentQuantity = 18,
            LastUpdated = DateTime.UtcNow
        },
        new InventoryItem
        {
            ItemId = 2,
            ItemName = "Office Monitor",
            CurrentQuantity = 7,
            LastUpdated = DateTime.UtcNow
        },
        new InventoryItem
        {
            ItemId = 3,
            ItemName = "Wireless Keyboard",
            CurrentQuantity = 32,
            LastUpdated = DateTime.UtcNow
        }
    ];

    public Task<IReadOnlyList<InventoryItem>> GetInventoryAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<InventoryItem> result = inventory.ToList();
        return Task.FromResult(result);
    }

    public Task<InventoryItem> UpdateQuantityAsync(
        int id,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        InventoryItem item = inventory.FirstOrDefault(inventoryItem => inventoryItem.ItemId == id)
            ?? throw new KeyNotFoundException($"Inventory item {id} was not found.");

        item.CurrentQuantity = quantity;
        item.LastUpdated = DateTime.UtcNow;

        return Task.FromResult(item);
    }
}
