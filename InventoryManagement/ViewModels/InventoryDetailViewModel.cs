using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InventoryManagement.Core.Models;
using InventoryManagement.Core.Services;
using InventoryManagement.Messages;
using Microsoft.Maui.Controls;

namespace InventoryManagement.ViewModels;

public sealed partial class InventoryDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly IInventoryService inventoryService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private InventoryItem? item;

    [ObservableProperty]
    private string quantityText = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;
    public InventoryDetailViewModel(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
        IncreaseQuantityCommand = new RelayCommand(() => ChangeQuantity(1));
        DecreaseQuantityCommand = new RelayCommand(() => ChangeQuantity(-1));
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    partial void OnItemChanged(InventoryItem? value)
    {
        if (value is not null)
        {
            QuantityText = value.CurrentQuantity.ToString();
        }

        OnPropertyChanged(nameof(Title));
    }

    public string Title => Item is null ? "Inventory item" : Item.ItemName;
    public IRelayCommand IncreaseQuantityCommand { get; }
    public IRelayCommand DecreaseQuantityCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Item", out object? value) && value is InventoryItem inventoryItem)
        {
            Item = inventoryItem;
        }
    }

    private void ChangeQuantity(int amount)
    {
        if (!int.TryParse(QuantityText, out int quantity))
        {
            quantity = 0;
        }

        QuantityText = Math.Max(0, quantity + amount).ToString();
    }

    private async Task SaveAsync()
    {
        InventoryItem? selectedItem = Item;
        if (IsBusy || selectedItem is null)
        {
            return;
        }

        if (!int.TryParse(QuantityText, out int quantity) || quantity < 0)
        {
            ErrorMessage = "Enter a quantity of zero or greater.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            InventoryItem updatedItem = await inventoryService.UpdateQuantityAsync(selectedItem.ItemId, quantity);
            WeakReferenceMessenger.Default.Send(new InventoryItemUpdatedMessage(updatedItem));
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
