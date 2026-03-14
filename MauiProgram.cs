using Microsoft.Extensions.Logging;
using ThePupCircle.MobileApp.Services;
using Microsoft.Maui.Handlers;

#if ANDROID
using ThePupCircle.MobileApp.Platforms.Android;
#endif
#if IOS
using ThePupCircle.MobileApp.Platforms.iOS;
#endif

namespace ThePupCircle.MobileApp;

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
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler<IWebView, CustomWebViewHandler>();
#endif
#if IOS
				handlers.AddHandler<IWebView, CustomWebViewHandler>();
#endif
			});

		// Register services
		builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
