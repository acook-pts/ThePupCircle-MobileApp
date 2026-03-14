using ThePupCircle.MobileApp.Services;

namespace ThePupCircle.MobileApp;

public partial class MainPage : ContentPage
{
	private static string BaseUrl => AppConstants.BaseUrl;
	private readonly IPushNotificationService _pushService;

	public MainPage(IPushNotificationService pushService)
	{
		_pushService = pushService;
		_pushService.TokenReceived += OnPushTokenReceived;

		if (_pushService is Services.PushNotificationService svc)
			svc.NotificationUrlReceived += OnNotificationUrlReceived;

		InitializeComponent();
		System.Diagnostics.Debug.WriteLine($"=== MainPage Constructor ===");
		System.Diagnostics.Debug.WriteLine($"BaseUrl: {BaseUrl}");
		LoadWebsite();

		// Initialise push notifications in the background — won't block page load
		_ = _pushService.InitializeAsync();
	}

	private void LoadWebsite()
	{
		try
		{
			System.Diagnostics.Debug.WriteLine($"LoadWebsite() called");
			System.Diagnostics.Debug.WriteLine($"Loading URL: {BaseUrl}");
			
			// Set the source but keep loading indicator visible
			// It will hide when OnWebViewNavigated fires
			webView.Source = BaseUrl;
			
			System.Diagnostics.Debug.WriteLine($"WebView.Source set successfully");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"LoadWebsite error: {ex.Message}");
			System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
			
			loadingIndicator.IsVisible = false;
			DisplayAlert("Connection Error", 
				$"Failed to load: {BaseUrl}\n\nError: {ex.Message}", 
				"OK");
		}
	}

	private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigating ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"NavigationEvent: {e.NavigationEvent}");
	}

	private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigated ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"Result: {e.Result}");

		if (e.Result == WebNavigationResult.Success)
		{
			System.Diagnostics.Debug.WriteLine("Navigation successful - showing WebView");
			loadingIndicator.IsVisible = false;
			webView.IsVisible = true;

			// If we already have a push token, inject it now
			var token = _pushService.GetToken();
			if (!string.IsNullOrEmpty(token))
				_ = InjectPushTokenAsync(token);
		}
		else
		{
			System.Diagnostics.Debug.WriteLine($"Navigation failed: {e.Result}");
			loadingIndicator.IsVisible = false;
			DisplayAlert("Load Failed",
				$"Failed to load page.\n\nURL: {e.Url}\nResult: {e.Result}\n\nCheck the Output window for details.",
				"OK");
		}
	}

	// Called when Firebase gives us a token (may fire after the page has already loaded)
	private void OnPushTokenReceived(object? sender, string token)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			if (webView.IsVisible)
				_ = InjectPushTokenAsync(token);
		});
	}

	// Called when the user taps a push notification that carries a 'url' data payload
	private void OnNotificationUrlReceived(object? sender, string url)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			System.Diagnostics.Debug.WriteLine($"=== Navigating to notification URL: {url} ===");
			webView.Source = url;
		});
	}

	private async Task InjectPushTokenAsync(string token)
	{
		try
		{
			// Expose the FCM token to the website so it can register with its backend.
			// The site should listen for: window.addEventListener('nativePushTokenReady', e => { ... })
			// or check window.nativePushToken on load.
			var safeToken = token.Replace("'", "\\'");
			var js = $"window.nativePushToken = '{safeToken}'; " +
			         $"window.dispatchEvent(new CustomEvent('nativePushTokenReady', {{ detail: {{ token: '{safeToken}' }} }}));";
			await webView.EvaluateJavaScriptAsync(js);
			System.Diagnostics.Debug.WriteLine("=== Push token injected into WebView ===");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"=== Failed to inject push token: {ex.Message} ===");
		}
	}
}
