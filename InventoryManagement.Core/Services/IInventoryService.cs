using InventoryManagement.Core.Models;

namespace InventoryManagement.Core.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItem>> GetInventoryAsync(CancellationToken cancellationToken = default);
    Task<InventoryItem> UpdateQuantityAsync(int id, int quantity, CancellationToken cancellationToken = default);
}
