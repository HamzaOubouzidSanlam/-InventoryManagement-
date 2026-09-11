using InventoryManagement.ViewModels;

namespace InventoryManagement.Views;

public partial class InventoryPage : ContentPage
{
    public InventoryPage(InventoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
