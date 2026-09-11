using CommunityToolkit.Mvvm.Messaging.Messages;
using InventoryManagement.Core.Models;

namespace InventoryManagement.Messages;

public sealed class InventoryItemUpdatedMessage(InventoryItem value) : ValueChangedMessage<InventoryItem>(value);
