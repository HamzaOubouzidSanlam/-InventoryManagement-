namespace InventoryManagement
{
    public partial class AppShell : Shell
    {
        public AppShell(
            Views.InventoryPage inventoryPage, 
            Views.InventoryDetailPage inventoryDetailPage
        )
        {
            InitializeComponent();
            Items.Add(new ShellContent
            {
                Title = "Inventory",
                Route = "InventoryPage",
                Content = inventoryPage
            });
            Routing.RegisterRoute(
                nameof(Views.InventoryDetailPage), 
                typeof(Views.InventoryDetailPage));
        }
    }
}
