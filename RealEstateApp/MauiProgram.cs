using Microsoft.Extensions.Logging;
using RealEstateApp.Repositories;
using RealEstateApp.Services;
using RealEstateApp.ViewModels;
using RealEstateApp.Views;
using ZXing.Net.Maui.Controls;
namespace RealEstateApp;

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
                fonts.AddFont("fa-solid-900.ttf", "FA-solid");
                fonts.AddFont("MaterialIcons-Regular.ttf", "mrIcons");
            });

        builder.Services.AddSingleton<IPropertyService, MockRepository>();
        builder.Services.AddSingleton<PropertyListPage>();
        builder.Services.AddSingleton<PropertyListPageViewModel>();

        builder.Services.AddTransient<PropertyDetailPage>();
        builder.Services.AddTransient<PropertyDetailPageViewModel>();
        builder.Services.AddTransient<AddEditPropertyPage>();
        builder.Services.AddTransient<AddEditPropertyPageViewModel>();
        builder.Services.AddTransient<CompassPage>();
        builder.Services.AddTransient<CompassPageViewModel>();
        builder.Services.AddTransient<HeightCalculatorPage>();
        builder.Services.AddTransient<HeightCalculatorPageViewModel>();
        builder.Services.AddTransient<ImageListPage>();
        builder.Services.AddTransient<ImageListPageViewModel>();

        builder.UseBarcodeReader();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
