using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using InventoryManagement.Core.Models;
using InventoryManagement.Core.Services;
using InventoryManagement.Messages;

namespace InventoryManagement.ViewModels;

public sealed partial class InventoryViewModel : ObservableObject
{
    private readonly IInventoryService inventoryService;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public InventoryViewModel(IInventoryService inventoryService)
    {
        this.inventoryService = inventoryService;
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        OpenDetailsCommand = new AsyncRelayCommand<InventoryItem?>(OpenDetailsAsync);
        WeakReferenceMessenger.Default.Register<InventoryItemUpdatedMessage>(this, static (recipient, message) =>
        {
            ((InventoryViewModel)recipient).ReplaceItem(message.Value);
        });
        _ = LoadAsync();
    }

    public ObservableCollection<InventoryItem> Items { get; } = [];
    public IAsyncRelayCommand LoadCommand { get; }
    public IAsyncRelayCommand<InventoryItem?> OpenDetailsCommand { get; }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            IReadOnlyList<InventoryItem> items = await inventoryService.GetInventoryAsync();
            Items.Clear();
            foreach (InventoryItem item in items)
            {
                Items.Add(item);
            }
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

    private static async Task OpenDetailsAsync(InventoryItem? item)
    {
        if (item is not null)
        {
            await Shell.Current.GoToAsync(nameof(InventoryManagement.Views.InventoryDetailPage), new Dictionary<string, object>
            {
                ["Item"] = item
            });
        }
    }

    private void ReplaceItem(InventoryItem updatedItem)
    {
        int index = Items.ToList().FindIndex(item => item.ItemId == updatedItem.ItemId);
        if (index >= 0)
        {
            Items[index] = updatedItem;
        }
    }
}
