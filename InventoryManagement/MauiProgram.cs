using Microsoft.Extensions.Logging;
using InventoryManagement.Core.Services;
using InventoryManagement.ViewModels;
using InventoryManagement.Views;

namespace InventoryManagement
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IInventoryService, FakeInventoryService>();
            builder.Services.AddTransient<InventoryViewModel>();
            builder.Services.AddTransient<InventoryDetailViewModel>();
            builder.Services.AddTransient<InventoryPage>();
            builder.Services.AddTransient<InventoryDetailPage>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
