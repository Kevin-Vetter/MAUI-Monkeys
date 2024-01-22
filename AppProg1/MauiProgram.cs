using AppProg1.Services;
using AppProg1.ViewModels;
using AppProg1.Views;

namespace AppProg1;

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
		builder.Services.AddSingleton<MonkeyService>();
		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MonkeyDetailsViewModel>();
		builder.Services.AddSingleton<MonkeyDetails>();

        return builder.Build();
	}
}
