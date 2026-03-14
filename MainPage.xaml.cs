namespace ThePupCircle.MobileApp;

public partial class MainPage : ContentPage
{
	private static string BaseUrl => AppConstants.BaseUrl;
	private string? _fcmToken;

	public MainPage()
	{
		InitializeComponent();
		System.Diagnostics.Debug.WriteLine($"=== MainPage Constructor ===");
		System.Diagnostics.Debug.WriteLine($"BaseUrl: {BaseUrl}");
		LoadWebsite();
	}

	private async Task InitFcmTokenAsync()
	{
#if ANDROID
		try
		{
			// Use cached token if available; Firebase refreshes it via OnNewToken
			_fcmToken = Microsoft.Maui.Storage.Preferences.Default.Get<string?>("fcm_token", null);

			if (string.IsNullOrEmpty(_fcmToken))
			{
				_fcmToken = await Plugin.Firebase.CloudMessaging.CrossFirebaseCloudMessaging.Current.GetTokenAsync();
				Microsoft.Maui.Storage.Preferences.Default.Set("fcm_token", _fcmToken);
			}

			System.Diagnostics.Debug.WriteLine($"FCM token: {_fcmToken?[..20]}...");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"FCM token error: {ex.Message}");
		}
#endif
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

	private async Task InjectFcmTokenAsync()
	{
		if (string.IsNullOrEmpty(_fcmToken)) return;

		// Inject the token into the page — the site listens for nativeFcmTokenReady and POSTs to the server
		var js = $@"
			(function() {{
				window.nativeFcmToken = '{_fcmToken}';
				window.dispatchEvent(new CustomEvent('nativeFcmTokenReady', {{ detail: '{_fcmToken}' }}));
			}})();";

		try
		{
			await webView.EvaluateJavaScriptAsync(js);
			System.Diagnostics.Debug.WriteLine("FCM token injected into WebView");
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"FCM token injection failed: {ex.Message}");
		}
	}

	private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigating ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"NavigationEvent: {e.NavigationEvent}");
	}

	private async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"=== WebView Navigated ===");
		System.Diagnostics.Debug.WriteLine($"URL: {e.Url}");
		System.Diagnostics.Debug.WriteLine($"Result: {e.Result}");

		if (e.Result == WebNavigationResult.Success)
		{
			System.Diagnostics.Debug.WriteLine("Navigation successful - showing WebView");
			loadingIndicator.IsVisible = false;
			webView.IsVisible = true;
			await InitFcmTokenAsync();
			await InjectFcmTokenAsync();
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
}
