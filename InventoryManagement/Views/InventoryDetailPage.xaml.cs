using InventoryManagement.ViewModels;

namespace InventoryManagement.Views;

public partial class InventoryDetailPage : ContentPage
{
    public InventoryDetailPage(InventoryDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
